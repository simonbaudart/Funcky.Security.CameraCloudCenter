// -----------------------------------------------------------------------
//  <copyright file="AuthenticationInformation.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Core.Model
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Information about the authentication of the user
    /// </summary>
    [DataContract]
    public class AuthenticationInformation
    {
        /// <summary>
        /// Gets or sets the login.
        /// </summary>
        /// <value>
        /// The login.
        /// </value>
        [DataMember(Name = "login")]
        public string Login { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [DataMember(Name = "password")]
        public string Password { get; set; }
    }
}