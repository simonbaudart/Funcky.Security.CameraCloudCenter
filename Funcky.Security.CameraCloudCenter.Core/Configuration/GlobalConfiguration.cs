// -----------------------------------------------------------------------
//  <copyright file="GlobalConfiguration.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Some global parameters
    /// </summary>
    [DataContract]
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
        /// Gets or sets the configurations.
        /// </summary>
        /// <value>
        /// The configurations.
        /// </value>
        [DataMember(Name = "cameras")]
        public CameraConfiguration[] Configurations { get; set; }

        /// <summary>
        /// Gets or sets the ff probe path.
        /// </summary>
        /// <value>
        /// The ff probe path.
        /// </value>
        [DataMember(Name = "ffprobePath")]
        public string FFProbePath { get; set; }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>
        /// The users.
        /// </value>
        [DataMember(Name = "users")]
        public List<User> Users { get; set; }
    }
}