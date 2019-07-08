// -----------------------------------------------------------------------
//  <copyright file="CameraConfiguration.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Core
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represent a configuration of a camera storage
    /// </summary>
    [DataContract]
    public class CameraConfiguration
    {
        /// <summary>
        /// Gets or sets the name of the camera.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the source directory.
        /// </summary>
        /// <value>
        /// The source directory.
        /// </value>
        [DataMember(Name = "sourceDirectory")]
        public string SourceDirectory { get; set; }
    }
}