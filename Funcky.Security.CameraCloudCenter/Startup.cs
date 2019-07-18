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

    using Funcky.Security.CameraCloudCenter.Core.Configuration;
    using Funcky.Security.CameraCloudCenter.Core.Exceptions;
    using Funcky.Security.CameraCloudCenter.Jobs;

    using Hangfire;
    using Hangfire.MemoryStorage;

    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
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
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfigurationRoot Configuration { get; private set; }

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

            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appsettings.json", true, true).AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);
            builder.AddEnvironmentVariables();
            this.Configuration = builder.Build();

            GlobalConfiguration.Instance = new GlobalConfiguration { FFProbePath = this.Configuration.GetConnectionString("ffprobe") };

            this.StartHangfire(app);

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
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
                {
                    options.Events.OnRedirectToLogin = context =>
                        {
                            context.Response.StatusCode = 401;    
                            return Task.CompletedTask;
                        };
                });

            services.AddHttpContextAccessor();
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
        /// Starts the hangfire.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <exception cref="System.ApplicationException">The configuration file {configurationFilePath} does not exists</exception>
        private void StartHangfire(IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = new[] { new EveryOneAuthorization() } });

            var configurationFilePath = this.Configuration.GetConnectionString("CameraConfigurations");

            if (!File.Exists(configurationFilePath))
            {
                throw new ApplicationException($"The configuration file {configurationFilePath} does not exists");
            }

            GlobalConfiguration.Instance.Configurations = JsonConvert.DeserializeObject<CameraConfiguration[]>(File.ReadAllText(configurationFilePath));

            this.EnsureConfiguration(GlobalConfiguration.Instance.Configurations);

            foreach (var configuration in GlobalConfiguration.Instance.Configurations)
            {
                RecurringJob.AddOrUpdate<CameraInputProcessor>($"PROCESS INPUT : {configuration.Key}", x => x.Process(configuration), Cron.Minutely(), TimeZoneInfo.Utc);
                RecurringJob.AddOrUpdate<CameraOutputCleanup>($"PROCESS CLEAN : {configuration.Key}", x => x.Process(configuration), Cron.Hourly(), TimeZoneInfo.Utc);
            }
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
    }
}