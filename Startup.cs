using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageScheduler.Data;
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
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using AutoMapper;
using Hangfire;
using Hangfire.SqlServer;

namespace MessageScheduler
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc();
            services.AddOptions<DbContextOptions<MessageSchedulerContext>>();
            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<IScheduledTextRepository, ScheduledTextRepository>();
            services.AddDbContext<MessageSchedulerContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionString"]);
            });          
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Message Scheduler API", Version = "v1" });
            });
            Hangfire.ConfigureService(Configuration["ConnectionString"], services);      
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IBackgroundJobClient backgroundJobs, MessageSchedulerContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            context.Database.EnsureCreated();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                c.RoutePrefix = string.Empty;
            });

            Hangfire.Configure(app);
            //backgroundJobs.Enqueue(() => new Workers.TestTextJob(Configuration).Execute());
            Hangfire.InitializeJobs();
        }
    }
}
