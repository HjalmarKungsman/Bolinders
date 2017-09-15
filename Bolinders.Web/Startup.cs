using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Bolinders.Web.Repositories;
using Microsoft.Extensions.Configuration;
using Bolinders.Web.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Bolinders.Web
{
    public class Startup
    {
        IConfigurationRoot Configuration;

        public Startup(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appsettings.json").Build();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var conn = Configuration.GetConnectionString("BolindersVehicles");
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(conn));
            services.AddTransient<IVehicleRepository, VehicleRepository>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext ctx)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }

            app.UseStaticFiles();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "pagination",
                    template: "vehicles/page/{page}",
                    defaults: new { Controller = "Vehicle", action = "List" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Vehicle}/{action=List}/{id?}");
            });

            Seed.FillIfEmpty(ctx);
        }
    }
}
