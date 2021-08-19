using Hangfire;
using Hangfire.Console;
using Hangfire.Pro.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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