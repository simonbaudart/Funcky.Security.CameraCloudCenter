// -----------------------------------------------------------------------
//  <copyright file="UrlHelperExtensions.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Extensions
{
    using System;
    using System.IO;
    using System.Linq;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Get the path with adding a crc to invalidate client cache
        /// </summary>
        /// <param name="helper">The current UrlHelper</param>
        /// <param name="path">The path of the file</param>
        /// <param name="env">The env.</param>
        /// <returns>
        /// path with adding a crc to invalidate client cache
        /// </returns>
        public static string GetPathWithCrc(this IUrlHelper helper, string path, IHostingEnvironment env)
        {
            var physicalPath = Path.Combine(env.WebRootPath, path.Replace("/", "\\"));
            var fileInfo = new FileInfo(physicalPath);

            if (!fileInfo.Exists)
            {
                return path;
            }

            if (path.IndexOf("?", StringComparison.Ordinal) == -1)
            {
                return path + "?v=" + fileInfo.LastWriteTimeUtc.Ticks;
            }

            return path + "&v=" + fileInfo.LastWriteTimeUtc.Ticks;
        }
    }
}