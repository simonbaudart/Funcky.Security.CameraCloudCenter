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

    using Core.Configuration;
    using Core.Model;

    using Isopoh.Cryptography.Argon2;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// All methods to access to the login
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Authorize]
    public class LoginController : Controller
    {
#if DEBUG

        /// <summary>
        /// Generates the hash.
        /// </summary>
        /// <param name="authentication">The authentication.</param>
        /// <returns>The hash and the salt to store in configuration</returns>
        [HttpPost]
        [Route("api/login/generate")]
        [AllowAnonymous]
        public ActionResult GenerateHash([FromBody] JObject authentication)
        {
            var password = authentication["password"]?.ToString();

            if (string.IsNullOrWhiteSpace(password))
            {
                return this.Unauthorized();
            }

            var result = new JObject();
            result["hash"] = Argon2.Hash(password);

            return this.Ok(result);
        }

#endif

        /// <summary>
        /// Logins the specified authentication.
        /// </summary>
        /// <param name="authentication">The authentication.</param>
        /// <returns>The result of the login</returns>
        [HttpPost]
        [Route("api/login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] AuthenticationInformation authentication)
        {
            if (string.IsNullOrWhiteSpace(authentication?.Login) || string.IsNullOrWhiteSpace(authentication.Password))
            {
                return this.Unauthorized();
            }

            var user = GlobalConfiguration.Instance.Users.SingleOrDefault(x => x.Email?.Equals(authentication.Login, StringComparison.InvariantCultureIgnoreCase) == true);

            if (user == null)
            {
                return this.Unauthorized();
            }

            if (!Argon2.Verify(user.Hash, authentication.Password))
            {
                return this.Unauthorized();
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, authentication.Login));

            var principal = new ClaimsPrincipal(identity);
            await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return this.Ok();
        }

        /// <summary>
        /// Logouts this user.
        /// </summary>
        /// <returns>204 no content</returns>
        [HttpPost]
        [Route("api/logout")]
        public async Task<ActionResult> Logout()
        {
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return this.Ok();
        }

        /// <summary>
        /// Determines whether this user is authenticated.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/isAuthenticated")]
        public ActionResult IsAuthenticated()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return this.Ok(new {authenticated = true});
            }

            return this.Unauthorized();
        }
    }
}