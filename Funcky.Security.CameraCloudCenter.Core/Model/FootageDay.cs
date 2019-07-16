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
        /// Gets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        [DataMember(Name = "date")]
        public string Date => this.FootageDate.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// Gets or sets the footage date.
        /// </summary>
        /// <value>
        /// The footage date.
        /// </value>
        [DataMember(Name = "footageDate")]
        public DateTime FootageDate { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [DataMember(Name = "title")]
        public string Title { get; set; }
    }
}