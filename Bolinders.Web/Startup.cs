﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Bolinders.Core.Models;
using Bolinders.Core.DataAccess;
using Bolinders.Core.Services;
using Microsoft.AspNetCore.Http;

namespace Bolinders.Web
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 2;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });


            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(150);
                options.LoginPath = "/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                options.AccessDeniedPath = "/Home/Index"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                options.SlidingExpiration = true;
            });

            // Add application services.
            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext ctx, UserManager<ApplicationUser> userManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {

                routes.MapRoute(
                    name: "pagination",
                    template: "bilar/sida/{page}",
                    defaults: new { Controller = "Vehicles", action = "List" });

                routes.MapRoute(
                    name: null,
                    template: "bilar",
                    defaults: new { Controller = "Vehicles", action = "List" });

                routes.MapRoute(
                    name: null,
                    template: "bilar/nya",
                    defaults: new { Controller = "Vehicles", action = "List", used = false });

                routes.MapRoute(
                    name: null,
                    template: "bilar/begagnade",
                    defaults: new { Controller = "Vehicles", action = "List", used = true });

                routes.MapRoute(
                    name: "Vehicles-routing",
                    template: "bilar/{action}/{id?}",
                    defaults: new { Controller = "Vehicles", action = "Details" });

                routes.MapRoute(
                    name: null,
                    template: "bil/{vehicleLinkId}",
                    defaults: new { Controller = "Vehicles", action = "Details", vehicleLinkId = "" });

                routes.MapRoute(
                    name: "Contact",
                    template: "kontakt",
                    defaults: new { Controller = "Home", Action = "Contact" });

                routes.MapRoute(
                    name: "Admin",
                    template: "admin",
                    defaults: new { Controller = "Account", Action = "Login" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

            });

            Seed.FillIfEmpty(ctx, userManager);
        }
    }
}
