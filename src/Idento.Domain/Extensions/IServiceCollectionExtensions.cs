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

using Idento.Domain;
using Idento.Domain.Stores;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static void AddIdentoDomain(this IServiceCollection service)
        {
            service.AddScoped<DataContext>();

            service.AddScoped<IApplicationStore, ApplicationStore>();
            service.AddScoped<IExternalLoginProviderStore, ExternalLoginProviderStore>();

            service.AddScoped<IRoleStore, RoleStore>();
            service.AddScoped<IUserStore, UserStore>();
        }
    }
}
