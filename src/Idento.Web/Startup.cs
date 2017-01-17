/*
 * Copyright (c) Wouter Huysentruit
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
using System.Threading.Tasks;
using Idento.Core.Cryptography;
using Idento.Domain.Models;
using Idento.Domain.Stores;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Idento.Web
{
    public class Startup
    {
        private readonly IHostingEnvironment env;

        public Startup(IHostingEnvironment env)
        {
            this.env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddIdento(o =>
            //{
            //    o.ConnectionString = Configuration["Data:DefaultConnection:ConnectionString"];
            //    o.RequireSsl = this.env.IsProduction();
            //});

            //services
            //    .AddMvc()
            //    .AddRazorOptions(o =>
            //    {
            //        o.ViewLocationExpanders.Add(new IdentoViewLocationExpander());
            //    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            ConfigureAuthentication(app).Wait();

            app.UseIdento();

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }

        private async Task ConfigureAuthentication(IApplicationBuilder app)
        {
            Application application;

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var applicationStore = serviceScope.ServiceProvider.GetService<IApplicationStore>();
                if (applicationStore == null) throw new Exception("No IApplicationStore service registered");
                application = await applicationStore.GetByDisplayName(IdentoConstants.ClientId);
                if (application == null)
                {
                    application = new Application
                    {
                        Enabled = true,
                        DisplayName = IdentoConstants.ClientId,
                        ClientId = Crypto.GenerateRandomToken(32),
                        ClientSecret = Crypto.GenerateRandomToken(64)
                    };
                    await applicationStore.Create(application);
                }
                application.RedirectUris = Configuration["Application:Url"] + "signin-oidc";
                await applicationStore.Update(application);

                using (var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<Role>>())
                {
                    var adminRole = await roleManager.FindByNameAsync(IdentoConstants.ManagerRoleName);
                    if (adminRole == null)
                    {
                        adminRole = new Role
                        {
                            ConcurrencyStamp = Guid.NewGuid().ToString(),
                            Name = IdentoConstants.ManagerRoleName
                        };
                        await roleManager.CreateAsync(adminRole);
                    }
                }

                using (var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>())
                {
                    var adminUser = await userManager.FindByNameAsync(IdentoConstants.AdminUserName);
                    if (adminUser == null)
                    {
                        adminUser = new User { UserName = IdentoConstants.AdminUserName };
                        await userManager.CreateAsync(adminUser, IdentoConstants.DefaultAdminPassword);
                    }

                    if (!await userManager.IsInRoleAsync(adminUser, IdentoConstants.ManagerRoleName))
                        await userManager.AddToRoleAsync(adminUser, IdentoConstants.ManagerRoleName);
                }
            }

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                AuthenticationScheme = IdentoConstants.ManagerCookieScheme,
                CookieHttpOnly = true,
                CookieSecure = CookieSecurePolicy.SameAsRequest,
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromDays(14),
                LoginPath = new PathString("/Home/Login"),
                LogoutPath = new PathString("/Home/Logout"),
                AccessDeniedPath = new PathString("/Home/AccessDenied"),
            });

            var openIdConnectOptions = new OpenIdConnectOptions
            {
                AuthenticationScheme = IdentoConstants.ManagerOidcScheme,
                AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet,
                ClientId = application.ClientId,
                Authority = Configuration["Application:Url"],
                PostLogoutRedirectUri = Configuration["Application:Url"],
                ResponseType = "id_token token",
                SignInScheme = IdentoConstants.ManagerCookieScheme,
                // TODO SecurityTokenValidator
            };

            openIdConnectOptions.Scope.Add(""); // TODO

            app.UseOpenIdConnectAuthentication(openIdConnectOptions);
        }
    }
}
