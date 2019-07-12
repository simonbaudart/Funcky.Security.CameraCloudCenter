// -----------------------------------------------------------------------
//  <copyright file="AzureConstants.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Core.Providers.AzureStorage
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// All the constants for Azure
    /// </summary>
    internal class AzureConstants
    {
        /// <summary>
        /// The container event
        /// </summary>
        internal const string ContainerEvent = "event";

        /// <summary>
        /// The container others
        /// </summary>
        internal const string ContainerOthers = "others";

        /// <summary>
        /// The container recording
        /// </summary>
        internal const string ContainerRecording = "recording";

        /// <summary>
        /// The container snap
        /// </summary>
        internal const string ContainerSnap = "snap";

        /// <summary>
        /// The empty footage URL
        /// </summary>
        internal const string EmptyFootageUrl = "https://via.placeholder.com/1920x1080.png&text=No%20footage";

        /// <summary>
        /// The footage date format
        /// </summary>
        internal const string FootageDateFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// The footage date meta data
        /// </summary>
        internal const string FootageDateMetaData = "FootageDate";

        /// <summary>
        /// The footage duration meta data
        /// </summary>
        internal const string FootageDurationMetaData = "FootageDuration";

        /// <summary>
        /// The minimum snap interval
        /// </summary>
        internal static TimeSpan MinimumSnapInterval => TimeSpan.FromSeconds(15);
    }
}