// -----------------------------------------------------------------------
//  <copyright file="FootageController.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Api
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Funcky.Security.CameraCloudCenter.Core.Configuration;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Manage request for footages
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiController]
    [Authorize]
    public class FootageController : ControllerBase
    {
        /// <summary>
        /// Gets the footages.
        /// </summary>
        /// <param name="cameraName">Name of the camera.</param>
        /// <param name="date">The date.</param>
        /// <returns>
        /// List of all days with footage for a camera
        /// </returns>
        [Route("api/footages/{cameraName}")]
        [HttpGet]
        public async Task<IActionResult> GetFootages(string cameraName, [FromQuery] string date)
        {
            if (!DateTime.TryParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var footageDate))
            {
                return this.BadRequest();
            }

            var configuration = GlobalConfiguration.Instance.Configurations.SingleOrDefault(x => x.Key == cameraName);

            if (configuration == null)
            {
                return this.NotFound();
            }

            var storageProvider = configuration.GetStorageProvider();
            var footages = await storageProvider.GetFootages(footageDate);

            return this.Ok(footages);
        }

        /// <summary>
        /// Gets the footage URL.
        /// </summary>
        /// <param name="cameraName">Name of the camera.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>The footage url</returns>
        [Route("api/footage/{cameraName}")]
        [HttpGet]
        public IActionResult GetFootageUrl(string cameraName, string id)
        {
            var configuration = GlobalConfiguration.Instance.Configurations.SingleOrDefault(x => x.Key == cameraName);

            if (configuration == null)
            {
                return this.NotFound();
            }

            var storageProvider = configuration.GetStorageProvider();
            var footageUrl = storageProvider.GetFootageUrl(id);

            return this.Ok(footageUrl);
        }
    }
}