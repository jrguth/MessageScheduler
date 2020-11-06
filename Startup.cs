using System;
using System.Collections.Generic;
using MessageScheduler.Data;
using MessageScheduler.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Dashboard;
using MessageScheduler.Domain;
using MessageScheduler.Workers;
using Microsoft.OpenApi.Models;
using MessageScheduler.Auth;
using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace MessageScheduler
{
    public class Startup
    {
        private const string DASHBOARD_TITLE = "Message Scheduler Jobs";
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
            services
                .AddAuthentication(ApiKeyDefaults.AuthenticationScheme)
                .AddApiKeyInHeaderOrQueryParams<ApiKeyProvider>(options =>
                {
                    options.Realm = "Message Scheduler API";
                    options.KeyName = "Authorization";
                });
            
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(ApiKeyDefaults.AuthenticationScheme)
                    .Build();
            });

            services
                .Configure<TwilioConfig>(Configuration.GetSection("Twilio"))
                .Configure<IpSafeListConfig>(Configuration.GetSection("IpSafeList"), options => options.BindNonPublicProperties = true);

            services.AddOptions<DbContextOptions<MessageSchedulerContext>>();
            services.AddAutoMapper(typeof(Startup));
            
            services.AddSingleton(Configuration);
            services.AddSingleton<IApiKeyRepository, ApiKeyConfigRepository>();
            services.AddSingleton<ISmsClient, TwilioClient>();
            services.AddSingleton<IRequestAuthStrategy, IpWhitelistAuthStrategy>();
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
                var apiKeyScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Required header for authorization. Header name: 'Authorization'",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "API Key Authorization"
                    }
                };
                options.AddSecurityDefinition("API Key Authorization", apiKeyScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        apiKeyScheme,
                        new string[] {}
                    }
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

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardFilter(app.ApplicationServices.GetRequiredService<IRequestAuthStrategy>()) }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
            });

            app.UseMiddleware<NonApiMiddleware>();
                   
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Message Scheduler API V1");
            });

            var testJob = new StartupTextJob(app.ApplicationServices.GetService<ISmsClient>(), Configuration);
            backgroundJobs.Enqueue(() => testJob.Execute());
            RecurringJob.AddOrUpdate<SendTextJob>(job => job.Execute(), Cron.Minutely);
        }
    }
}
