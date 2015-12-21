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

using Autofac;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using Idento.Core.IdentityServer3.Implementations;

namespace Idento.Core.IdentityServer3
{
    internal class IdentityServerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ScopeStore>().As<IScopeStore>().InstancePerLifetimeScope();
            builder.RegisterType<ClientStore>().As<IClientStore>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<ViewLoader>().As<IViewLoader>().SingleInstance();
        }
    }
}
