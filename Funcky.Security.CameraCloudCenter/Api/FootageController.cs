// -----------------------------------------------------------------------
//  <copyright file="FootageController.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Funcky.Security.CameraCloudCenter.Core.Model;

    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Manage request for footages
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiController]
    public class FootageController : ControllerBase
    {
        /// <summary>
        /// Gets the footages.
        /// </summary>
        /// <param name="cameraName">Name of the camera.</param>
        /// <returns>List of all days with footage for a camera</returns>
        [Route("api/footages/{cameraName}")]
        [HttpGet]
        public List<FootageDay> GetFootages(string cameraName)
        {
            return new List<FootageDay>();
        }
    }
}