// -----------------------------------------------------------------------
//  <copyright file="IFootageStorage.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Core.Providers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface to manage footage storage
    /// </summary>
    public interface IFootageStorage
    {
        /// <summary>
        /// Cleanups the old footages.
        /// </summary>
        /// <returns>The task to wait for in async</returns>
        Task Cleanup();

        /// <summary>
        /// Uploads the file to the storage.
        /// </summary>
        /// <param name="localPath">The local path.</param>
        /// <returns>The task to wait for in async</returns>
        Task UploadFile(string localPath);
    }
}