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

    using Funcky.Security.CameraCloudCenter.Core.Configuration;
    using Funcky.Security.CameraCloudCenter.Jobs;

    using Hangfire;
    using Hangfire.MemoryStorage;

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

            foreach (var configuration in GlobalConfiguration.Instance.Configurations)
            {
                RecurringJob.AddOrUpdate<CameraInputProcessor>($"PROCESS INPUT : {configuration.Name}", x => x.Process(configuration), Cron.Minutely(), TimeZoneInfo.Utc);
                RecurringJob.AddOrUpdate<CameraOutputCleanup>($"PROCESS CLEAN : {configuration.Name}", x => x.Process(configuration), Cron.Hourly(), TimeZoneInfo.Utc);
            }
        }
    }
}