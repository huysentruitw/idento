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

using Idento.Configuration;
using Idento.Core.Configuration;
using Microsoft.Owin.Cors;

namespace Owin
{
    public static class IdentoAppBuilderExtensions
    {
        public static void UseIdento(this IAppBuilder app, IdentoOptions options)
        {
            app.UseCors(CorsOptions.AllowAll);
            options.Validate();
            app.UseAutofacMiddleware(new DependencyContainerBuilder(options).Build());

            // The order is very important, if we call UseIdentityServer first, then models posted to
            // the manager Web API are not converted anymore.
            app.Map("/api", apiApp => apiApp.UseIdentoWebApi());
            app.UseIdentityServer(options);
        }
    }
}
