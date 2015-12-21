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
using Autofac.Integration.Owin;
using IdentityServer3.Core.Services;
using Microsoft.Owin;

namespace Idento.Core
{
    internal static class DependencyResolverExtensions
    {
        public static T ResolveFromAutofacOwinLifetimeScope<T>(this IDependencyResolver resolver)
        {
            var owin = resolver.Resolve<OwinEnvironmentService>();
            var context = new OwinContext(owin.Environment);
            var scope = context.GetAutofacLifetimeScope();
            return scope.Resolve<T>();
        }
    }
}
