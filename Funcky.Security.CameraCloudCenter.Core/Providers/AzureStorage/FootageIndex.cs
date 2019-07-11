// -----------------------------------------------------------------------
//  <copyright file="FootageIndex.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Core.Providers.AzureStorage
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// To avoid storage in another database but to be able to quickly retrieve some information
    /// this classe is used and stored like a database in azure storage
    /// </summary>
    /// <remarks>
    /// Why not SQL ? Because à want to be DB free and only be dependant of the storage
    /// </remarks>
    [DataContract]
    public class FootageIndex
    {
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
    }
}