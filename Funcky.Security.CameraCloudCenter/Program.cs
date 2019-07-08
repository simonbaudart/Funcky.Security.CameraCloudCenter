// -----------------------------------------------------------------------
//  <copyright file="Program.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    /// <summary>
    /// Start the applixation
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Creates the web host builder.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The web host builder for this application</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
        }

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
    }
}