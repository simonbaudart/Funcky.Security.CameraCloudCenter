// -----------------------------------------------------------------------
//  <copyright file="CameraOutputCleanup.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Jobs
{
    using System;
    using System.IO;
    using System.Linq;

    using Funcky.Security.CameraCloudCenter.Core.Configuration;
    using Funcky.Security.CameraCloudCenter.Core.OutputManager;

    /// <summary>
    /// Clean each camera output
    /// </summary>
    public class CameraOutputCleanup
    {
        /// <summary>
        /// The lock camera output
        /// </summary>
        private static readonly object LockCameraOutput = new object();

        /// <summary>
        /// Processes the specified camera configuration.
        /// </summary>
        /// <param name="cameraConfiguration">The camera configuration.</param>
        public void Process(CameraConfiguration cameraConfiguration)
        {
            var azureOutputManager = cameraConfiguration.AzureOutput == null ? null : new AzureOutputManager(cameraConfiguration.AzureOutput);

            lock (LockCameraOutput)
            {
                azureOutputManager?.Cleanup().Wait();
            }
        }
    }
}