// -----------------------------------------------------------------------
//  <copyright file="AzureOutputManager.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Core.OutputManager
{
    using System;
    using System.Linq;

    using Funcky.Security.CameraCloudCenter.Core.Configuration;

    /// <summary>
    /// Output to Azure
    /// </summary>
    public class AzureOutputManager
    {
        /// <summary>
        /// The azure output configuration
        /// </summary>
        private readonly AzureOutputConfiguration azureOutputConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureOutputManager"/> class.
        /// </summary>
        /// <param name="azureOutputConfiguration">The azure output configuration.</param>
        public AzureOutputManager(AzureOutputConfiguration azureOutputConfiguration)
        {
            this.azureOutputConfiguration = azureOutputConfiguration;
        }

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="localPath">The local path.</param>
        public void UploadFile(string localPath)
        {

        }
    }
}