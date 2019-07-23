// -----------------------------------------------------------------------
//  <copyright file="LoginController.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Api
{
    using System;
    using System.Linq;

    using Funcky.Security.CameraCloudCenter.Core.Model;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// All methods to access to the login
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    public class LoginController : Controller
    {
        /// <summary>
        /// Logins the specified authentication.
        /// </summary>
        /// <param name="authentication">The authentication.</param>
        /// <returns>The result of the login</returns>
        [HttpPost]
        [Route("api/login")]
        [AllowAnonymous]
        public ActionResult Login(AuthenticationInformation authentication)
        {
            var result = authentication.Login == authentication.Password;

            if (result)
            {
                return this.Ok();
            }

            return this.Unauthorized();
        }
    }
}