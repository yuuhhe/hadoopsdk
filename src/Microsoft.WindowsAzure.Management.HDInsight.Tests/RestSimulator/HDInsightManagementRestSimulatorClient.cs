﻿// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License.  You may obtain a copy
// of the License at http://www.apache.org/licenses/LICENSE-2.0
// 
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED
// WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT.
// 
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.RestSimulator
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.HDInsight.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.RestClient;

    internal class HDInsightManagementRestSimulatorClient : DisposableObject, IHDInsightManagementRestClient
    {
        private readonly IConnectionCredentials credentials;
        private readonly IDictionary<string, X509Certificate2> certificates = new Dictionary<string, X509Certificate2>();
        private readonly List<Guid> subscriptions = new List<Guid>();

        // List of Clusters stored.
        // Includes the expected 'tsthdx00hdxcibld02' cluster
        private static readonly Collection<CreateClusterRequest> PendingCreateClusters = new Collection<CreateClusterRequest>();
        private static readonly Collection<ListClusterContainerResult> PendingDeleteClusters = new Collection<ListClusterContainerResult>();
        private static readonly Collection<ListClusterContainerResult> Clusters = new Collection<ListClusterContainerResult>()
        {
            new ListClusterContainerResult(IntegrationTestBase.TestCredentials.DnsName, ClusterState.Running.ToString())
            {
                ConnectionUrl = @"https://" + IntegrationTestBase.TestCredentials.DnsName + ".azurehdinsight.net",
                CreatedDate = DateTime.UtcNow,
                Location = "East US",
                ResultError = null,
                UserName = "sa-po-svc",
                WorkerNodesCount = 4
            }
        };

        internal HDInsightManagementRestSimulatorClient(IConnectionCredentials credentials)
        {
            var cert = new X509Certificate2(IntegrationTestBase.TestCredentials.Certificate);
            this.certificates.Add(cert.Thumbprint, cert);
            this.subscriptions.Add(IntegrationTestBase.TestCredentials.SubscriptionId);
            this.credentials = credentials;
        }

        public async Task<string> ListCloudServices()
        {
            ValidateConnection();

            string value;
            lock (Clusters)
            {
                value = PayloadConverter.SerializeListContainersResult(Clusters, this.credentials.DeploymentNamespace);

                // Advances the state of the clusters. Uses tempList to mark clusters for deletion
                var tempList = new Collection<ListClusterContainerResult>();
                foreach (var cluster in Clusters)
                {
                    switch (cluster.ParsedState)
                    {
                        case ClusterState.Accepted:
                            cluster.ChangeState(ClusterState.ClusterStorageProvisioned);
                            break;
                        case ClusterState.ClusterStorageProvisioned:
                            cluster.ChangeState(ClusterState.AzureVMConfiguration);
                            break;
                        case ClusterState.AzureVMConfiguration:
                            cluster.ChangeState(ClusterState.HDInsightConfiguration);
                            break;
                        case ClusterState.HDInsightConfiguration:
                            cluster.ChangeState(ClusterState.Operational);
                            break;
                        case ClusterState.Operational:
                            cluster.ChangeState(ClusterState.Running);
                            break;
                        case ClusterState.DeleteQueued:
                            cluster.ChangeState(ClusterState.Deleting);
                            break;
                        case ClusterState.Deleting:
                            tempList.Add(cluster);
                            break;
                        case ClusterState.Error:
                        case ClusterState.Running:
                        case ClusterState.Unknown:
                            // NO-OP
                            break;
                    }
                }
                foreach (var cluster in tempList)
                {
                    Clusters.Remove(cluster);
                }

                // Moves the pending clusters to the actual Cluster list
                foreach (var pendingCluster in PendingCreateClusters)
                {

                    // If there are errors on the request, update ResultError
                    var cluster = new ListClusterContainerResult(pendingCluster.DnsName, ClusterState.Accepted.ToString())
                    {
                        ConnectionUrl = string.Format(@"https://{0}.azurehdinsight.net", pendingCluster.DnsName),
                        CreatedDate = DateTime.UtcNow,
                        Location = pendingCluster.Location,
                        ResultError = ValidateClusterCreation(pendingCluster),
                        UserName = pendingCluster.ClusterUserName,
                        WorkerNodesCount = pendingCluster.WorkerNodeCount
                    };

                    cluster.ChangeState(cluster.ResultError == null
                                            ? ClusterState.Accepted
                                            : ClusterState.Unknown);
                    Clusters.Add(cluster);
                }
                foreach (var cluster in PendingDeleteClusters)
                {
                    cluster.ChangeState(ClusterState.DeleteQueued);
                    Clusters.Add(cluster);
                }
                PendingCreateClusters.Clear();
                PendingDeleteClusters.Clear();
            }

            return await Task.FromResult(value);
        }

        private ClusterErrorStatus ValidateClusterCreation(CreateClusterRequest cluster)
        {
            if (!ValidateClusterCreationMetadata(cluster.HiveMetastore, cluster.OozieMetastore))
                return new ClusterErrorStatus(400, "Invalid metastores", "create");
            return null;
        }


        private bool ValidateClusterCreationMetadata(ComponentMetastore hive, ComponentMetastore oozie)
        {
            if (hive == null && oozie == null)
                return true;
            if (hive == null || oozie == null)
                return false;

            try
            {
                foreach (var metastore in new ComponentMetastore[] { hive, oozie })
                {
                    SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder
                    {
                        DataSource = metastore.Server,
                        InitialCatalog = metastore.Database,
                        UserID = metastore.User,
                        Password = metastore.Password,
                        IntegratedSecurity = false,
                    };

                    using (var connection = new SqlConnection(connectionString.ConnectionString))
                    {
                        connection.Open();
                        connection.Close();
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task CreateContainer(string dnsName, string location, string clusterPayload)
        {
            ValidateConnection();

            lock (Clusters)
            {
                var existingCluster = Clusters.FirstOrDefault(c => c.DnsName == dnsName);
                if (existingCluster != null)
                    throw new ConfigurationErrorsException("cluster already exists");

                var cluster = PayloadConverter.DeserializeClusterCreateRequest(clusterPayload);
                PendingCreateClusters.Add(cluster);
            }
            await Task.Delay(TimeSpan.FromMilliseconds(1));
        }
        public async Task DeleteContainer(string dnsName, string location)
        {
            ValidateConnection();

            lock (Clusters)
            {
                var cluster = Clusters.FirstOrDefault(c => c.DnsName == dnsName && c.Location == location);
                if (cluster == null)
                    throw new HDInsightRestClientException(HttpStatusCode.NotFound, "Cluster Not Found");

                PendingDeleteClusters.Add(cluster);
            }
            await Task.Delay(TimeSpan.FromMilliseconds(1));
        }

        private void ValidateConnection()
        {
            if (credentials.Certificate == null || credentials.SubscriptionId.Equals(Guid.Empty) ||
                !this.certificates.ContainsKey(credentials.Certificate.Thumbprint) ||
                !this.subscriptions.Contains(credentials.SubscriptionId))
            {
                throw new HDInsightRestClientException(HttpStatusCode.Forbidden, string.Empty);
            }
        }
    }
}