﻿using AspNetCoreHero.ToastNotification;
using FoodOrder.Areas.Admin.Repository;
using FoodOrder.Data;
using FoodOrder.Models;
using FoodOrder.Services.VnPay;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            var builderRazer = builder.Services.AddRazorPages();
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            // Configure DbContext
            builder.Services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectDB")));

            builder.Services.AddTransient<IEmailSendercs, EmailSender>();

            builder.Services.AddIdentity<AppUserModel, IdentityRole>()
           .AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();


            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                // options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                //options.Lockout.MaxFailedAccessAttempts = 5;
                //options.Lockout.AllowedForNewUsers = true;

                // User settings.
                //options.User.AllowedUserNameCharacters =
                //"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                //options.User.RequireUniqueEmail = false;
            });
            // Configure ToastNotification
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddNotyf(config =>
            {
                config.DurationInSeconds = 3;
                config.IsDismissable = true;
                config.Position = NotyfPosition.TopRight;
            });
            // configuration Login Google account
            //builder.Services.AddAuthentication().AddFacebook(
            //    opt =>
            //    {
            //        opt.ClientId = "",
            //        opt.ClientSecret= ""
            //    });
            builder.Services.AddAuthentication(options =>
            {
                //options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            }).AddCookie().AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
                options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;

            });

            builder.Services.AddAuthentication().AddFacebook(opt =>
 {
     
 });


      builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
     
   
 .AddCookie("ShipperScheme", options =>
 {
     options.LoginPath = "/Shipper/Account/Login";
     options.LogoutPath = "/Shipper/Account/login";
     options.AccessDeniedPath = "/Shipper/Account/Login";
 });

            // Configure Authorization Policies
            builder.Services.AddAuthorization(options =>
            {
                
                options.AddPolicy("ShipperPolicy", policy =>
                    policy.RequireClaim("Role", "Shipper")); ;
            });
            //Connect VNPay API
            builder.Services.AddScoped<IVnPayServices, VnPayServices>();

            var app = builder.Build();
            app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                builderRazer.AddRazorRuntimeCompilation();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
    name: "Areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
    name: "Areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
            app.MapControllerRoute(
         name: "category",
         pattern: "/category/{Slug}",
          defaults: new { controller = "Category", action = "Index" });
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
