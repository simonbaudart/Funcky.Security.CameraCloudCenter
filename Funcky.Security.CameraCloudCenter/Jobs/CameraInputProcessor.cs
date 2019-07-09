// -----------------------------------------------------------------------
//  <copyright file="CameraInputProcessor.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Jobs
{
    using System;
    using System.IO;
    using System.Linq;

    using Funcky.Security.CameraCloudCenter.Core.Configuration;

    /// <summary>
    /// Class that manage an input of a security camera
    /// </summary>
    public class CameraInputProcessor
    {
        /// <summary>
        /// The lock camera input
        /// </summary>
        private static readonly object LockCameraInput = new object();

        /// <summary>
        /// Processes the specified camera configuration.
        /// </summary>
        /// <param name="cameraConfiguration">The camera configuration.</param>
        public void Process(CameraConfiguration cameraConfiguration)
        {
            var storageProvider = cameraConfiguration.GetStorageProvider();

            lock (LockCameraInput)
            {
                foreach (var file in Directory.GetFiles(cameraConfiguration.SourceDirectory, "*.*", SearchOption.AllDirectories))
                {
                    storageProvider.UploadFile(file).Wait();

                    File.Delete(file);
                }
            }
        }
    }
}