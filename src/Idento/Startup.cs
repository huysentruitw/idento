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
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Idento.Core.Cryptography;
using Idento.Domain;
using Idento.Domain.Models;
using Idento.Domain.Stores;
using Idento.Helpers;
using Idento.Services;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Authentication.OpenIdConnect;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdento(o =>
            {
                o.SigningCertificate = typeof(Startup).GetTypeInfo().Assembly.LoadCertificateFromResource("Idento.Idento.pfx", "IdentoTest");
                o.ConnectionString = Configuration["Data:DefaultConnection:ConnectionString"];
            });

            services.AddMvc();

            // For the UI
            services
                .AddMvc()
                .AddRazorOptions(o =>
                {
                    o.ViewLocationExpanders.Add(new IdentoViewLocationExpander());
                });

            // TODO Move mappings to separate file(s)
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Application, ManagerUI.Applications.Models.ListItem>();
                cfg.CreateMap<Application, ManagerUI.Applications.Models.EditOrCreate>().ReverseMap();

                cfg.CreateMap<ExternalLoginProvider, ManagerUI.LoginProviders.Models.ListItem>()
                    .Include<ExternalLoginProvider, ManagerUI.LoginProviders.Models.OAuth2ListItem>()
                    .Include<ExternalLoginProvider, ManagerUI.LoginProviders.Models.WsFederationListItem>()
                    .ForMember(dst => dst.Provider, x => x.MapFrom(src => EnumExtensions.GetDisplayName(src.Provider)));
                cfg.CreateMap<ExternalLoginProvider, ManagerUI.LoginProviders.Models.OAuth2ListItem>()
                    .ForMember(dst => dst.OAuth2ClientSecret, x => x.MapFrom(src => src.OAuth2ClientSecret == null ? "" : src.OAuth2ClientSecret.Substring(0, 6) + "..."));
                cfg.CreateMap<ExternalLoginProvider, ManagerUI.LoginProviders.Models.WsFederationListItem>();
                cfg.CreateMap<ExternalLoginProvider, ManagerUI.LoginProviders.Models.EditOrCreate>().ReverseMap();

                cfg.CreateMap<User, ManagerUI.Users.Models.ListItem>();
                cfg.CreateMap<User, ManagerUI.Users.Models.Create>().ReverseMap();
            });
            services.AddSingleton<IMapper>(x => mapperConfig.CreateMapper());

            // Add application services.
            services.AddScoped<ManagerUI.Applications.ApplicationsService>();
            services.AddScoped<ManagerUI.LoginProviders.LoginProvidersService>();
            services.AddScoped<ManagerUI.Users.UsersService>();
            services.AddScoped<LoginUI.Login.LoginService>();
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

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
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                        serviceScope.ServiceProvider.GetService<DataContext>().Database.Migrate();
                }
                catch
                {
                }
            }

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());
            
            ConfigureAuthentication(app).Wait();

            app.UseIdento();

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);

        private async Task ConfigureAuthentication(IApplicationBuilder app)
        {
            Application application;

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var applicationStore = serviceScope.ServiceProvider.GetService<IApplicationStore>();
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

            app.UseCookieAuthentication(o =>
            {
                o.AutomaticAuthenticate = true;
                o.AutomaticChallenge = true;
                o.AuthenticationScheme = IdentoConstants.ManagerCookieScheme;
                o.CookieHttpOnly = true;
                o.CookieSecure = CookieSecureOption.SameAsRequest;
                o.SlidingExpiration = true;
                o.ExpireTimeSpan = TimeSpan.FromDays(14);
                o.LoginPath = new PathString("/Home/Login");
                o.LogoutPath = new PathString("/Home/Logout");
                o.AccessDeniedPath = new PathString("/Home/AccessDenied");
            });

            app.UseOpenIdConnectAuthentication(o =>
            {
                o.AuthenticationScheme = IdentoConstants.ManagerOidcScheme;
                o.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
                o.ClientId = application.ClientId;
                o.Authority = Configuration["Application:Url"];
                o.PostLogoutRedirectUri = Configuration["Application:Url"];
                o.ResponseType = "id_token token";
                o.Scope.Add(""); // TODO
                o.SignInScheme = IdentoConstants.ManagerCookieScheme;
                // TODO o.SecurityTokenValidator
            });
        }
    }
}
