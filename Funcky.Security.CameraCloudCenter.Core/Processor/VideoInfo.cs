// -----------------------------------------------------------------------
//  <copyright file="VideoInfo.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Core.Processor
{
    using System;
    using System.IO;
    using System.Linq;

    using Funcky.Security.CameraCloudCenter.Core.Configuration;

    using NReco.VideoInfo;

    /// <summary>
    /// Get info about a video
    /// </summary>
    public class VideoInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VideoInfo" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        public VideoInfo(FileInfo file)
        {
            this.File = file;
        }

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <value>
        /// The file.
        /// </value>
        public FileInfo File { get; }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <returns>The duration of the video file</returns>
        public TimeSpan GetDuration()
        {
            try
            {
                var ffProbe = new FFProbe();
                ffProbe.FFProbeExeName = GlobalConfiguration.Instance.FFProbePath;
                var videoInfo = ffProbe.GetMediaInfo(this.File.FullName);
                return videoInfo.Duration;
            }
            catch (AggregateException)
            {
                // This can be caused by an empty file
                return TimeSpan.Zero;
            }
        }
    }
}