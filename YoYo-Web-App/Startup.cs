using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using YoYo_Web_App.MiddlewareExtensions;
using YoYo_Web_App.Models.AppConfig;
using YoYo_Web_App.Models.Yoyo;

namespace YoYo_Web_App
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var config = new AppConfig();
            _config.Bind("AppConfig", config);

            services.AddSingleton<IAppConfig>(config);
            services.AddSingleton<IYoyoRepository, YoyoRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddControllersWithViews();
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseConfigureExceptionHandler();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "yoyo",
                    pattern: "{controller}/{action}/{id:int}/{actionType}",
                    constraints: new { actionType = "^(Warned|Completed)$", });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Yoyo}/{action=YoyoView}/{id?}");

                endpoints.MapHealthChecks("/healthy");

                endpoints.Map("/env", context => context.Response.WriteAsync("Environment - " + env.EnvironmentName));
            });
        }
    }
}
