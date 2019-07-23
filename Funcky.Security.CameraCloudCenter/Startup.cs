// -----------------------------------------------------------------------
//  <copyright file="Startup.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using AspNetCoreRateLimit;

    using Funcky.Security.CameraCloudCenter.Core.Configuration;
    using Funcky.Security.CameraCloudCenter.Core.Exceptions;
    using Funcky.Security.CameraCloudCenter.Jobs;

    using Hangfire;
    using Hangfire.MemoryStorage;

    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Newtonsoft.Json;

    using GlobalConfiguration = Funcky.Security.CameraCloudCenter.Core.Configuration.GlobalConfiguration;

    /// <summary>
    /// Manage the application startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }


        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfiguration Configuration { get; private set; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var configurationFilePath = this.Configuration.GetConnectionString("ConfigFile");

            if (!File.Exists(configurationFilePath))
            {
                throw new ApplicationException($"The configuration file {configurationFilePath} does not exists");
            }

            GlobalConfiguration.Instance = JsonConvert.DeserializeObject<GlobalConfiguration>(File.ReadAllText(configurationFilePath));

            this.StartHangfire(app);

            app.UseIpRateLimiting();

            app.UseAuthentication();
            app.UseMvc();

            app.UseStaticFiles();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(
                    options =>
                        {
                            options.Events.OnRedirectToLogin = context =>
                                {
                                    context.Response.StatusCode = 401;
                                    return Task.CompletedTask;
                                };
                        });

            services.AddMemoryCache();
            services.AddOptions();
            services.AddHttpContextAccessor();

            this.ConfigureIpRateLimit(services);

            services.AddMvc();

            this.ConfigureHangfire(services);
        }

        /// <summary>
        /// Configures the hangfire.
        /// </summary>
        /// <param name="services">The services.</param>
        private void ConfigureHangfire(IServiceCollection services)
        {
            services.AddHangfire(x => x.UseMemoryStorage());
            services.AddHangfireServer();
        }

        /// <summary>
        /// Configures the ip rate limit.
        /// </summary>
        /// <param name="services">The services.</param>
        private void ConfigureIpRateLimit(IServiceCollection services)
        {
            services.Configure<IpRateLimitOptions>(this.Configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(this.Configuration.GetSection("IpRateLimitPolicies"));

            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        /// <summary>
        /// Ensures the configuration by performing checks to be sure that configuration is correct
        /// </summary>
        /// <param name="configurations">The configurations to check.</param>
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private void EnsureConfiguration(CameraConfiguration[] configurations)
        {
            // Each configuration must have a key
            if (configurations.Any(x => string.IsNullOrWhiteSpace(x.Key)))
            {
                throw new ConfigurationException("All camera configuration must have a key");
            }

            // And all key must be unique
            if (configurations.Any(x => configurations.Count(c => c.Key == x.Key) != 1))
            {
                throw new ConfigurationException("All camera configuration must have a unique key");
            }
        }

        /// <summary>
        /// Starts the hangfire.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <exception cref="System.ApplicationException">The configuration file {configurationFilePath} does not exists</exception>
        private void StartHangfire(IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = new[] { new EveryOneAuthorization() } });

            this.EnsureConfiguration(GlobalConfiguration.Instance.Configurations);

            foreach (var configuration in GlobalConfiguration.Instance.Configurations)
            {
                RecurringJob.AddOrUpdate<CameraInputProcessor>($"PROCESS INPUT : {configuration.Key}", x => x.Process(configuration), Cron.Minutely(), TimeZoneInfo.Utc);
                RecurringJob.AddOrUpdate<CameraOutputCleanup>($"PROCESS CLEAN : {configuration.Key}", x => x.Process(configuration), Cron.Hourly(), TimeZoneInfo.Utc);
            }
        }
    }
}