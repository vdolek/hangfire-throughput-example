using Hangfire;
using Hangfire.Console;
using Hangfire.Pro.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace HangfireTest
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire((serviceProvider, config) =>
            {
                config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseRedisStorage(
                        "localhost:6379",
                        new RedisStorageOptions
                        {
                            Prefix = "hangfire-test"
                        })
                    .UseConsole();
            });

            services.AddHangfireServer();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHangfireDashboard(string.Empty, new DashboardOptions
            {
                AppPath = null
            });
            
            RecurringJob.AddOrUpdate<AllJob>(x => x.RunAllJob(default, default), "0 0 1 * *");
        }
    }
}