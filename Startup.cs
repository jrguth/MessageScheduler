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
            services.AddMvc();

            services.Configure<TwilioConfig>(Configuration.GetSection("Twilio"));

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
                });
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Message Scheduler API V1");
            });

            app.UseHangfireDashboard();       


            var testJob = new TestTextJob(app.ApplicationServices.GetService<ISmsClient>(), Configuration);
            backgroundJobs.Enqueue(() => testJob.Execute());
            RecurringJob.AddOrUpdate<SendTextJob>(job => job.Execute(), Cron.Minutely);
        }
    }
}
