using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageScheduler.Data;
using MessageScheduler.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Dashboard;
using MessageScheduler.Domain;
using MessageScheduler.Workers;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace MessageScheduler
{
    public class Startup
    {
        private const string AUTH_POLICY_NAME = "HangfireDashboardPolicy";
        private const string DASHBOARD_TITLE = "Message Scheduler Jobs";
        private const string DASHBOARD_DISPLAY_NAME = "MessageScheduler";

        public Startup(IConfiguration configuration, IHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc();
            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => Configuration.Bind("AzureAd", options))
                .AddAzureADBearer(options => Configuration.Bind("AzureAd", options));

            services
                .Configure<TwilioConfig>(Configuration.GetSection("Twilio"))
                .Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
                {
                    options.Authority += "/v2.0";
                    options.TokenValidationParameters.ValidateIssuer = false;
                })
                .Configure<JwtBearerOptions>(AzureADDefaults.JwtBearerAuthenticationScheme, options =>
                {
                    options.Authority += "/v2.0";
                    options.TokenValidationParameters.ValidateIssuer = false;
                });

            services.AddOptions<DbContextOptions<MessageSchedulerContext>>();
            services.AddAutoMapper(typeof(Startup));
            
            services.AddSingleton(Configuration);
            services.AddSingleton<ISmsClient, TwilioClient>();
            services.AddScoped<IScheduledTextRepository, ScheduledTextRepository>();
            services.AddDbContext<MessageSchedulerContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionString"]);
            });
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Message Scheduler API"
                });
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{Configuration["AzureAd:Instance"]}/common/oauth2/v2.0/authorize", UriKind.Absolute),
                            TokenUrl = new Uri($"{Configuration["AzureAd:Instance"]}/common/oauth2/v2.0/token", UriKind.Absolute),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "Sign In Permissions" },
                                { "profile", "User Profile Permissions" },
                                { $"api://{Configuration["AzureAd:ClientId"]}/access_as_user", "Application API Permissions" }
                            }
                        }                   }

                });
            });
        
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AUTH_POLICY_NAME, builder =>
                {
                    builder
                        .AddAuthenticationSchemes(AzureADDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser();
                });
            });

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration["ConnectionString"], new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));
            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IBackgroundJobClient backgroundJobs, MessageSchedulerContext context)
        {
            if (HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            context.Database.Migrate();

            app.UseHttpsRedirection();


            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard("/dashboard", new DashboardOptions
                {
                    Authorization = new List<IDashboardAuthorizationFilter> { },
                    DashboardTitle = DASHBOARD_TITLE
                })
                .RequireAuthorization(AUTH_POLICY_NAME);
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Message Scheduler API V1");
                c.OAuthClientId(Configuration["AzureAd:ClientID"]);
            });
            app.UseHangfireDashboard();       


            var testJob = new TestTextJob(app.ApplicationServices.GetService<ISmsClient>(), Configuration);
            backgroundJobs.Enqueue(() => testJob.Execute());
            RecurringJob.AddOrUpdate<SendTextJob>(job => job.Execute(), Cron.Minutely);
        }
    }
}
