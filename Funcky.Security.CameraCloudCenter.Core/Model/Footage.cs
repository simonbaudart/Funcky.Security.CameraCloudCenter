// -----------------------------------------------------------------------
//  <copyright file="Footage.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represent a day of footage
    /// </summary>
    [DataContract]
    public class Footage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Footage" /> class.
        /// </summary>
        public Footage()
        {
            this.Sequences = new List<Footage>();
        }

        /// <summary>
        /// Gets the end date.
        /// </summary>
        /// <value>
        /// The end.
        /// </value>
        [DataMember(Name = "end")]
        public string End => this.FootageEndDate.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// Gets or sets the footage date.
        /// </summary>
        /// <value>
        /// The footage date.
        /// </value>
        [DataMember(Name = "footageDate")]
        public DateTime FootageDate { get; set; }

        /// <summary>
        /// Gets or sets the footage end date.
        /// </summary>
        /// <value>
        /// The footage end date.
        /// </value>
        [DataMember(Name = "footageEndDate")]
        public DateTime FootageEndDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the sequences.
        /// </summary>
        /// <value>
        /// The sequences.
        /// </value>
        [DataMember(Name = "sequences")]
        public List<Footage> Sequences { get; set; }

        /// <summary>
        /// Gets the start date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        [DataMember(Name = "start")]
        public string Start => this.FootageDate.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the type of the footage.
        /// </summary>
        /// <value>The Type.</value>
        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}