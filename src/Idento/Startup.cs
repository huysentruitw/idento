/*
 * Copyright 2016 Wouter Huysentruit
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using Idento.Domain;
using Idento.Services;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Authentication.OpenIdConnect;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Idento
{
    public class Startup
    {
        private readonly IHostingEnvironment env;

        public Startup(IHostingEnvironment env)
        {
            this.env = env;

            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // TODO For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdento(o =>
            {
                o.SigningCertificate = Certificate.LoadFromResrouce("Idento.Idento.pfx", "IdentoTest");
                o.ConnectionString = Configuration["Data:DefaultConnection:ConnectionString"];
            });

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // TODO For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                try
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        serviceScope.ServiceProvider.GetService<DataContext>()
                             .Database.Migrate();
                    }
                }
                catch { }
            }

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            #region Configure OpenIdConnect Authentication
            app.UseCookieAuthentication(o =>
            {
                o.AuthenticationScheme = "IdentoCookie";
                o.ExpireTimeSpan = TimeSpan.FromHours(10);  // TODO make configurable
                o.SlidingExpiration = false;                // TODO make configurable
                o.CookieSecure = CookieSecureOption.Always;
                //o.SessionStore = sessionStore;
            });

            app.UseOpenIdConnectAuthentication(o =>
            {
                o.AuthenticationScheme = "IdentoOpenId";
                o.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
                o.ClientId = "myclientid";
                o.Authority = "https://localhost:44395/";   // TODO
                o.PostLogoutRedirectUri = "https://localhost:44395/";   // TODO
                o.ResponseType = "id_token token";
                o.Scope.Add(""); // TODO
                o.SignInScheme = "IdentoCookie";
                //o.SecurityTokenValidator
            });
            #endregion

            app.UseIdento();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
