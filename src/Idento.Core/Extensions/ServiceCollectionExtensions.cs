/*
 * Copyright 2017 Wouter Huysentruit
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
using IdentityModel;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Idento.Core.AspNetIdentity;
using Idento.Core.Configuration;
using Idento.Core.IdentityServer.Services;
using Idento.Core.IdentityServer.Stores;
using Idento.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddIdento(this IServiceCollection service, Action<IdentoOptions> setupAction)
        {
            var options = new IdentoOptions();
            setupAction?.Invoke(options);
            options.Validate();

            service.AddIdentoDomain();

            service.AddIdentoDomainSql(o =>
            {
                o.ConnectionString = options.ConnectionString;
            });

            service.AddScoped<IPasswordHasher<User>, PasswordHasher>();

            service.AddIdentity<User, Role>(o =>
            {
                o.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
                o.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
            })
                .AddIdentoStores()
                .AddDefaultTokenProviders();

            var builder = service.AddIdentityServer(o =>
            {
                // TODO where are those options now?
                //o.AuthenticationOptions = new AuthenticationOptions
                //{
                //    EnableSignOutPrompt = false,
                //};
                //o.RequireSsl = options.RequireSsl;
            });
            builder.Services.AddScoped<IClientStore, ClientStore>();
            builder.Services.AddScoped<IResourceStore, ResourceStore>();
            builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<ISigningCredentialStore, SigningCredentialStore>();

            //service.AddIdentoApi();
        }
    }
}