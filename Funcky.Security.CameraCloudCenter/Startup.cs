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

    using Core.Configuration;
    using Core.Exceptions;

    using Hangfire;
    using Hangfire.MemoryStorage;

    using Jobs;

    using Joonasw.AspNetCore.SecurityHeaders;

    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Newtonsoft.Json;

    using GlobalConfiguration = Core.Configuration.GlobalConfiguration;

    /// <summary>
    /// Manage the application startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
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
        private IConfiguration Configuration { get; }

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

            this.ConfigureHangfire(app);
            this.ConfigureSecurity(app, env);
            this.ConfigureWeb(app);
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

            this.ConfigureServiceIpRateLimit(services);
            this.ConfigureServiceSecurity(services);

            services.AddMvc();

            this.ConfigureServiceHangfire(services);
        }

        /// <summary>
        /// Starts the hangfire.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <exception cref="System.ApplicationException">The configuration file {configurationFilePath} does not exists</exception>
        private void ConfigureHangfire(IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions {Authorization = new[] {new EveryOneAuthorization()}});

            this.EnsureConfiguration(GlobalConfiguration.Instance.Configurations);

            foreach (var configuration in GlobalConfiguration.Instance.Configurations)
            {
                RecurringJob.AddOrUpdate<CameraInputProcessor>($"PROCESS INPUT : {configuration.Key}", x => x.Process(configuration), Cron.Minutely(), TimeZoneInfo.Utc);
                RecurringJob.AddOrUpdate<CameraOutputCleanup>($"PROCESS CLEAN : {configuration.Key}", x => x.Process(configuration), Cron.Hourly(), TimeZoneInfo.Utc);
            }
        }

        /// <summary>
        /// Configure all things related to security for the application
        /// </summary>
        /// <param name="app">The application builder used to configure</param>
        /// <param name="env">Then executing environment</param>
        private void ConfigureSecurity(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseHsts(new HstsOptions(TimeSpan.FromDays(30)));
            }

            app.UseXFrameOptions(new XFrameOptionsOptions {HeaderValue = XFrameOptionsOptions.XFrameOptionsValues.SameOrigin});

            app.UseXContentTypeOptions(new XContentTypeOptionsOptions {AllowSniffing = false});

            app.UseXXssProtection(new XXssProtectionOptions {EnableProtection = true, EnableAttackBlock = true});
            
            app.UseCsp(csp =>
            {
                csp.ByDefaultAllow.FromSelf();
                csp.AllowScripts.FromSelf()
                    .From("'sha256-ox311wuoHk7Vrx1uEa8PnZt34XhOUHuWdkjGrJLOmPM='")
                    .From("code.jquery.com")
                    .From("unpkg.com");
                csp.AllowStyles.FromSelf()
                    .From("fonts.googleapis.com")
                    .From("unpkg.com");
                csp.AllowFonts.FromSelf()
                    .From("data:")
                    .From("fonts.gstatic.com");
                csp.AllowImages.FromSelf()
                    .From("data:")
                    .From("*.blob.core.windows.net");
                csp.AllowAudioAndVideo.FromSelf()
                    .From("*.blob.core.windows.net");
            });
        }

        /// <summary>
        /// Configures the hangfire.
        /// </summary>
        /// <param name="services">The services.</param>
        private void ConfigureServiceHangfire(IServiceCollection services)
        {
            services.AddHangfire(x => x.UseMemoryStorage());
            services.AddHangfireServer();
        }

        /// <summary>
        /// Configures the ip rate limit.
        /// </summary>
        /// <param name="services">The services.</param>
        private void ConfigureServiceIpRateLimit(IServiceCollection services)
        {
            services.Configure<IpRateLimitOptions>(this.Configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(this.Configuration.GetSection("IpRateLimitPolicies"));

            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        /// <summary>
        /// Configure all services related to security
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureServiceSecurity(IServiceCollection services)
        {
            services.AddCsp();
        }

        /// <summary>
        /// Configure all part of the web
        /// </summary>
        /// <param name="app">The application builder</param>
        private void ConfigureWeb(IApplicationBuilder app)
        {
            app.UseIpRateLimiting();

            app.UseAuthentication();
            app.UseMvc();

            app.UseStaticFiles();
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