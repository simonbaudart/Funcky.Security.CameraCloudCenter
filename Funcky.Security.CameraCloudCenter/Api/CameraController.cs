// -----------------------------------------------------------------------
//  <copyright file="CameraController.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Funcky.Security.CameraCloudCenter.Core.Configuration;
    using Funcky.Security.CameraCloudCenter.Core.Model;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Controller to accces the cameras
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiController]
    [Authorize]
    public class CameraController : ControllerBase
    {
        /// <summary>
        /// Gets the cameras.
        /// </summary>
        /// <returns>List of all cameras</returns>
        [Route("api/cameras")]
        [HttpGet]
        public async Task<List<Camera>> GetCameras()
        {
            var cameras = new List<Camera>();

            foreach (var configuration in GlobalConfiguration.Instance.Configurations)
            {
                var storage = configuration.GetStorageProvider();
                var camera = new Camera(configuration);
                await storage.FillLastFootage(camera);

                cameras.Add(camera);
            }

            return cameras;
        }
    }
}