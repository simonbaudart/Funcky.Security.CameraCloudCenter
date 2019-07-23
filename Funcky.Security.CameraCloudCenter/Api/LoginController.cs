// -----------------------------------------------------------------------
//  <copyright file="LoginController.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Api
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Funcky.Security.CameraCloudCenter.Core.Model;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
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
        [HttpPut]
        [Route("api/login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody]AuthenticationInformation authentication)
        {
            if (string.IsNullOrWhiteSpace(authentication?.Login) || string.IsNullOrWhiteSpace(authentication.Password))
            {
                return this.Unauthorized();
            }

            var result = authentication.Login == authentication.Password;

            if (result)
            {
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, authentication.Login));

                var principal = new ClaimsPrincipal(identity);
                await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return this.Ok();
            }

            return this.Unauthorized();
        }
    }
}