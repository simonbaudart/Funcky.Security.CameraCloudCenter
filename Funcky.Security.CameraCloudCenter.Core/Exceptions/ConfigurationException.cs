// -----------------------------------------------------------------------
//  <copyright file="ConfigurationException.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Core.Exceptions
{
    using System;
    using System.Linq;

    /// <summary>
    /// Exception that occurs when the configuration is incorrect
    /// </summary>
    /// <seealso cref="System.ApplicationException" />
    public class ConfigurationException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException" /> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public ConfigurationException(string message)
            : base(message)
        {
        }
    }
}