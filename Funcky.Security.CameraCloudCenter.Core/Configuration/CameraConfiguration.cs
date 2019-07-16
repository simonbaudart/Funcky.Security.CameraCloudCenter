// -----------------------------------------------------------------------
//  <copyright file="CameraConfiguration.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Core.Configuration
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;

    using Funcky.Security.CameraCloudCenter.Core.Providers;
    using Funcky.Security.CameraCloudCenter.Core.Providers.AzureStorage;
    using Funcky.Security.CameraCloudCenter.Providers.AzureStorage;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represent a configuration of a camera storage
    /// </summary>
    [DataContract]
    public class CameraConfiguration
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        [DataMember(Name = "key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the name of the camera.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the source directory.
        /// </summary>
        /// <value>
        /// The source directory.
        /// </value>
        [DataMember(Name = "sourceDirectory")]
        public string SourceDirectory { get; set; }

        /// <summary>
        /// Gets or sets the azure output.
        /// </summary>
        /// <value>
        /// The azure output.
        /// </value>
        [DataMember(Name = "storageConfiguration")]
        public JObject StorageConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the type of the storage.
        /// </summary>
        /// <value>
        /// The type of the storage.
        /// </value>
        [DataMember(Name = "storageType")]
        public string StorageType { get; set; }

        /// <summary>
        /// Gets the storage provider.
        /// </summary>
        /// <returns>The storage provider associated to the storage type</returns>
        /// <exception cref="System.NotImplementedException">The storage type {this.StorageType} is not implemented</exception>
        public IFootageStorage GetStorageProvider()
        {
            switch (this.StorageType)
            {
                case "azure-storage":
                    return new AzureStorageProvider(this.StorageConfiguration.ToObject<AzureStorageConfiguration>());
            }

            throw new NotImplementedException($"The storage type {this.StorageType} is not implemented");
        }
    }
}