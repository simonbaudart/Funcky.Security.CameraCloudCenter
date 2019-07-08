// -----------------------------------------------------------------------
//  <copyright file="EveryOneAuthorization.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Jobs
{
    using System;
    using System.Linq;

    using Hangfire.Dashboard;

    /// <summary>
    /// Authorize everyone
    /// </summary>
    /// <seealso cref="Hangfire.Dashboard.IDashboardAuthorizationFilter" />
    public class EveryOneAuthorization : IDashboardAuthorizationFilter
    {
        /// <inheritdoc />
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}