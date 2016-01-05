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

using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using Idento.Configuration;
using Idento.Core;

namespace Owin
{
    internal static class IdentityServerAppBuilderExtensions
    {
        public static void UseIdentityServer(this IAppBuilder app, IdentoOptions options)
        {
            app.UseIdentityServer(new IdentityServerOptions
            {
                SigningCertificate = options.SigningCertificate,
                Factory = new IdentityServerServiceFactory
                {
                    ScopeStore = new Registration<IScopeStore>(dr => dr.ResolveFromAutofacOwinLifetimeScope<IScopeStore>()),
                    ClientStore = new Registration<IClientStore>(dr => dr.ResolveFromAutofacOwinLifetimeScope<IClientStore>()),
                    UserService = new Registration<IUserService>(dr => dr.ResolveFromAutofacOwinLifetimeScope<IUserService>()),
                    ViewService = new DefaultViewServiceRegistration(new DefaultViewServiceOptions
                    {
                        ViewLoader = new Registration<IViewLoader>(dr => dr.ResolveFromAutofacOwinLifetimeScope<IViewLoader>()),
                        CacheViews = true
                    })
                },
                LoggingOptions = new LoggingOptions
                {
                    EnableHttpLogging = options.EnableLogging,
                    EnableKatanaLogging = options.EnableLogging,
                    EnableWebApiDiagnostics = options.EnableLogging,
                    WebApiDiagnosticsIsVerbose = options.EnableLogging
                },

                AuthenticationOptions = new AuthenticationOptions
                {
                    //IdentityProviders = 
                    EnablePostSignOutAutoRedirect = true
                },

                RequireSsl = options.RequireSsl,
                EnableWelcomePage = false
            });
        }
    }
}
