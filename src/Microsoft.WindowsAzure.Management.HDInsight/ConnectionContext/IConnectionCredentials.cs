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

namespace Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext
{
    using System;
    using System.Security.Cryptography.X509Certificates;

    // TODO: REPLACE THIS CLASS WITH THE AZURE ONE

    /// <summary>
    /// Connection logic.
    /// </summary>
    public interface IConnectionCredentials
    {
        /// <summary>
        /// Gets the Azure Deployment Endpoint to talk to.
        /// </summary>
        Uri Endpoint { get; }

        /// <summary>
        /// Gets the HDInsight Namespace for Azure.
        /// </summary>
        string DeploymentNamespace { get; }

        /// <summary>
        /// Gets the subscription ID to be used.
        /// </summary>
        Guid SubscriptionId { get; }

        /// <summary>
        /// Gets the certificate to be used.
        /// </summary>
        X509Certificate2 Certificate { get; }
    }
}