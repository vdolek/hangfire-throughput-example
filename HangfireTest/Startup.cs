using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard;
using Hangfire.Pro.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HangfireTest
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire((serviceProvider, config) =>
            {
                config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    ////.UseSimpleAssemblyNameTypeSerializer()
                    ////.UseBatches()
                    ////.UseRecommendedSerializerSettings()
                    .UseRedisStorage(
                        "localhost:6379",
                        new RedisStorageOptions
                        {
                            Prefix = "hangfire-test",
                            InvisibilityTimeout = TimeSpan.FromMinutes(1)
                        })
                    ////.UseSerilogLogProvider()
                    .UseConsole()
                    ////.UseFilter(new ProlongExpirationTimeAttribute(settings.Hangfire.JobExpiration) { Order = 0 })
                    ////.UseFilter(new PerformContextCaptureFilter { Order = 1 })
                    ////.UseFilter(new LoggingFilter(serviceProvider.GetRequiredService<ILogger<LoggingFilter>>()) { Order = 2 })
                    .UseFilter(new AutomaticRetryAttribute { Attempts = 0, LogEvents = false });
            });

            // Add the processing server as IHostedService
            services.AddHangfireServer(options =>
            {
                ////options.SchedulePollingInterval = TimeSpan.FromSeconds(1);
                ////options.CancellationCheckInterval = TimeSpan.FromSeconds(1);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseHangfireDashboard(string.Empty, new DashboardOptions
            {
                AppPath = null
            });
            
            RecurringJob.AddOrUpdate<AllJob>(x => x.RunAllJob(default, default), "0 0 1 * *");
        }
    }
}