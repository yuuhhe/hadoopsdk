// Copyright (c) Microsoft Corporation
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

namespace Microsoft.WindowsAzure.Management.HDInsight.Client
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.WindowsAzure.Management.Framework;

    /// <inheritdoc />
    public class HDInsightSyncClientFactory : IHDInsightSyncClientFactory
    {
        /// <inheritdoc />
        public IHDInsightSyncClient Create(Guid subscriptionId, X509Certificate2 certificate)
        {
            return new HDInsightSyncClient(subscriptionId, certificate);
        }

        // TODO:
        ///// <inheritdoc />
        //public IHDInsightClient Create(Guid subscriptionId, X509Certificate2 certificate, Uri endpoint)
        //{
        //    return new HDInsightClient(subscriptionId, certificate, endpoint);
        //}
    }
}
