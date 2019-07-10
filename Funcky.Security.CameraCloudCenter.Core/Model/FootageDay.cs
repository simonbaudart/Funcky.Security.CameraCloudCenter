// -----------------------------------------------------------------------
//  <copyright file="FootageDay.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Core.Model
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represent a day of footage
    /// </summary>
    [DataContract]
    public class FootageDay
    {
        /// <summary>
        /// Gets or sets the footage date.
        /// </summary>
        /// <value>
        /// The footage date.
        /// </value>
        [DataMember(Name = "footageDate")]
        public DateTime FootageDate { get; set; }
    }
}