/*
 * Copyright 2016-2018 Wouter Huysentruit
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
using Idento.Domain.Entities;
using Idento.Domain.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Idento.Domain
{
    public static class IServiceCollectionExtensions
    {
        public static void AddIdentoDomain(this IServiceCollection services, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            services.AddDbContext<DataContext>(o => o.UseSqlServer(connectionString));

            services.AddScoped<IApplicationStore, Stores.ApplicationStore>();
            services.AddScoped<ICertificateStore, Stores.CertificateStore>();
            services.AddScoped<IUserApplicationStore, Stores.UserApplicationsStore>();
            services.AddScoped<IRoleStore<Role>, Stores.RoleStore>();
            services.AddScoped<IUserStore<User>, Stores.UserStore>();
            
            services.AddIdentity<User, Role>();
        }
    }
}