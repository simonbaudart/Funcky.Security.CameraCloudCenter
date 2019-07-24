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
    using System.Security.Cryptography;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Extensions of UrlHelper
    /// </summary>
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

            string hash;

            using (var sha256 = SHA256.Create())
            {
                hash = BitConverter.ToString(sha256.ComputeHash(File.ReadAllBytes(fileInfo.FullName)));
                hash = hash.Replace("-", string.Empty).ToLowerInvariant();
            }

            if (path.IndexOf("?", StringComparison.Ordinal) == -1)
            {
                return path + "?v=" + hash;
            }

            return path + "&v=" + hash;
        }
    }
}