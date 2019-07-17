// -----------------------------------------------------------------------
//  <copyright file="Camera.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Core.Model
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;

    using Funcky.Security.CameraCloudCenter.Core.Configuration;

    /// <summary>
    /// Represent a camera
    /// </summary>
    [DataContract]
    public class Camera
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Camera" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Camera(CameraConfiguration configuration)
        {
            this.Name = configuration.Name;
            this.Key = configuration.Key;
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        [DataMember(Name = "key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the last footage date.
        /// </summary>
        /// <value>
        /// The last footage date.
        /// </value>
        [DataMember(Name = "lastFootageDate")]
        public DateTime LastFootageDate { get; set; }

        /// <summary>
        /// Gets or sets the last footage image.
        /// </summary>
        /// <value>
        /// The last footage image.
        /// </value>
        [DataMember(Name = "lastFootageImage")]
        public string LastFootageImage { get; set; }

        /// <summary>
        /// Gets or sets the last footage image URL.
        /// </summary>
        /// <value>
        /// The last footage image URL.
        /// </value>
        [DataMember(Name = "lastFootageImageUrl")]
        public string LastFootageImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}