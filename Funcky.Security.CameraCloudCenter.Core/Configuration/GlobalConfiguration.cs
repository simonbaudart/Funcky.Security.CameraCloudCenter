// -----------------------------------------------------------------------
//  <copyright file="GlobalConfiguration.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Core.Configuration
{
    using System;
    using System.Linq;

    /// <summary>
    /// Some global parameters
    /// </summary>
    public class GlobalConfiguration
    {
        /// <summary>
        /// Gets or sets the configuration for this instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static GlobalConfiguration Instance { get; set; }

        /// <summary>
        /// Gets or sets the ff probe path.
        /// </summary>
        /// <value>
        /// The ff probe path.
        /// </value>
        public string FFProbePath { get; set; }
    }
}