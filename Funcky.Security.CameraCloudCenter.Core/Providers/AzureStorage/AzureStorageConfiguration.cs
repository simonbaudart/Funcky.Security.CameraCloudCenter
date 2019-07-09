// -----------------------------------------------------------------------
//  <copyright file="AzureStorageConfiguration.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Providers.AzureStorage
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represent the configuration of an azure storage
    /// </summary>
    [DataContract]
    public class AzureStorageConfiguration
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        [DataMember(Name = "connectionString")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        /// <value>
        /// The container.
        /// </value>
        [DataMember(Name = "container")]
        public string Container { get; set; }

        /// <summary>
        /// Gets or sets the retention.
        /// </summary>
        /// <value>
        /// The retention.
        /// </value>
        [DataMember(Name = "retention")]
        public int Retention { get; set; }
    }
}