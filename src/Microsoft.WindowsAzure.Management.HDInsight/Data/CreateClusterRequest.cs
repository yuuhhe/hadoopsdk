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

namespace Microsoft.WindowsAzure.Management.HDInsight.Data
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Object that encapsulates all the properties of a List Request.
    /// </summary>
    public class CreateClusterRequest
    {
        /// <summary>
        /// Gets or sets the DnsName of the cluster.
        /// </summary>
        public string DnsName { get; set; }

        /// <summary>
        /// Gets or sets the datacenter location for the cluster.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the StorageName for the default Azure Storage Account.
        /// This account will be used for schemaless paths and the cluster will 
        /// leverage to store some cluster level files.
        /// </summary>
        public string DefaultAsvAccountName { get; set; }

        /// <summary>
        /// Gets or sets the StorageKey for the default Azure Storage Account.
        /// This account will be used for schemaless paths and the cluster will 
        /// leverage to store some cluster level files.
        /// </summary>
        public string DefaultAsvAccountKey { get; set; }

        /// <summary>
        /// Gets or sets the StorageContainer for the default Azure Storage Account.
        /// This account will be used for schemaless paths and the cluster will 
        /// leverage to store some cluster level files.
        /// </summary>
        public string DefaultAsvContainer { get; set; }

        /// <summary>
        /// Gets or sets the login for the cluster's user.
        /// </summary>
        public string ClusterUserName { get; set; }

        /// <summary>
        /// Gets or sets the password for the cluster's user.
        /// </summary>
        public string ClusterUserPassword { get; set; }

        /// <summary>
        /// Gets or sets the number of workernodes for the cluster.
        /// </summary>
        public int WorkerNodeCount { get; set; }

        /// <summary>
        /// Gets additional Azure Storage Account that you want to enable access to.
        /// </summary>
        public Collection<AsvAccountConfiguration> AsvAccounts { get; private set; }

        /// <summary>
        /// Gets or sets the database to store the metadata for Oozie.
        /// </summary>
        public ComponentMetastore OozieMetastore { get; set; }

        /// <summary>
        /// Gets or sets the database to store the metadata for Hive.
        /// </summary>
        public ComponentMetastore HiveMetastore { get; set; }

        /// <summary>
        /// Initializes a new instance of the CreateClusterRequest class.
        /// </summary>
        public CreateClusterRequest()
        {
            this.AsvAccounts = new Collection<AsvAccountConfiguration>();
        }
    }
}