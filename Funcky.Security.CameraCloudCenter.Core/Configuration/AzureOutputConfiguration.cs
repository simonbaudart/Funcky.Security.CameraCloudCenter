// -----------------------------------------------------------------------
//  <copyright file="AzureOutputConfiguration.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Core.Configuration
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represent the configuration of an azure storage
    /// </summary>
    [DataContract]
    public class AzureOutputConfiguration
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
    }
}