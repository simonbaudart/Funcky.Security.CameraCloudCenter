// -----------------------------------------------------------------------
//  <copyright file="CameraOutputCleanup.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Jobs
{
    using System;
    using System.Linq;

    using Funcky.Security.CameraCloudCenter.Core.Configuration;

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
            var storageProvider = cameraConfiguration.GetStorageProvider();

            lock (LockCameraOutput)
            {
                storageProvider?.Cleanup().Wait();
            }
        }
    }
}