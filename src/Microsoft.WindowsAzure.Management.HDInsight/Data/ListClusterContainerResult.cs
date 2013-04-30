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
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Object that encapsulates all the properties of a List Request.
    /// </summary>
    public class ListClusterContainerResult
    {
        /// <summary>
        /// Gets the DnsName of the cluster.
        /// </summary>
        public string DnsName { get; private set; }

        /// <summary>
        /// Gets the a string value of the state of the cluster.
        /// </summary>
        public string State { get; private set; }

        /// <summary>
        /// Gets the parsed value of the state of the cluster
        /// Note: For compatibility reassons, any new or null states will revert to "UNKOWN".
        /// but the value will be preserved in State.
        /// </summary>
        public ClusterState ParsedState { get; private set; }

        /// <summary>
        /// Gets a possible error state for the cluster (if exists).
        /// </summary>
        public ClusterErrorStatus ResultError { get; internal set; }

        /// <summary>
        /// Gets the CreateDate of the cluster.
        /// </summary>
        public DateTime CreatedDate { get; internal set; }

        /// <summary>
        /// Gets the connection URL for the cluster.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", 
            Justification = "Value coming from the server. Value of parsing vs value of breaking is not worth it")]
        public string ConnectionUrl { get; internal set; }

        /// <summary>
        /// Gets the login username for the cluster.
        /// </summary>
        public string UserName { get; internal set; }

        /// <summary>
        /// Gets the Datacenter location of the cluster.
        /// </summary>
        public string Location { get; internal set; }

        /// <summary>
        /// Gets the count of worker nodes.
        /// </summary>
        public int WorkerNodesCount { get; internal set; }
        
        internal ListClusterContainerResult(string dnsName, string state)
        {
            this.DnsName = dnsName;
            this.State = state;
            ClusterState parsedState;
            this.ParsedState = (state == null || !Enum.TryParse(state, true, out parsedState))
                                ? ClusterState.Unknown
                                : parsedState;
        }

        internal void ChangeState(ClusterState newState)
        {
            this.State = newState.ToString();
            this.ParsedState = newState;
        }
    }
}