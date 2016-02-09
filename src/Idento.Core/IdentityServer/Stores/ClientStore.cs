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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Core.Models;
using IdentityServer4.Core.Services;
using Idento.Domain.Models;
using Idento.Domain.Stores;

namespace Idento.Core.IdentityServer.Stores
{
    internal class ClientStore : IClientStore
    {
        private IApplicationStore store;

        public ClientStore(IApplicationStore store)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            this.store = store;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var application = await this.store.GetByClientId(clientId);
            if (application == null || !application.Enabled)
                return null;

            var redirectUris = SplitValues(application.RedirectUris);
            var allowedScopes = SplitValues(application.AllowedScopes ?? "");
            var allowedExternalLoginProviders = SplitValues(application.AllowedExternalLoginProviders ?? "");

            var allowedCorsOrigins = SplitValues(application.AllowedCorsOrigins ?? "");
            allowedCorsOrigins.AddRange(redirectUris); // Also allow redirect URIs as CORS origins

            return new Client
            {
                ClientId = application.ClientId,
                ClientName = application.DisplayName,
                Enabled = application.Enabled,
                ClientUri = application.Uri,
                RedirectUris = redirectUris,
                PostLogoutRedirectUris = redirectUris,

                AccessTokenType = AccessTokenType.Jwt,
                AccessTokenLifetime = application.AccessTokenLifetimeInMinutes * 60,
                IdentityTokenLifetime = (int)TimeSpan.FromMinutes(5).TotalSeconds,
                AuthorizationCodeLifetime = (int)TimeSpan.FromMinutes(5).TotalSeconds,

                Flow = Translate(application.Flow),
                RequireConsent = application.RequireConsent,
                AllowRememberConsent = true,

                AllowAccessToAllScopes = application.AllowAllScopes,
                AllowedScopes = allowedScopes,

                AllowedCorsOrigins = allowedCorsOrigins,
                IdentityProviderRestrictions = allowedExternalLoginProviders,
            };
        }

        /// <summary>
        /// Translate CogniStreamer IdentityServer OAuth2Flow to ThinkTecture IdentityServer3 Flows enum.
        /// </summary>
        /// <param name="flow">The CongniStreamer IdentityServer OAuth2Flow enumeration.</param>
        /// <returns>The ThinkTecture IdentityServer3 variant.</returns>
        private static Flows Translate(OAuth2Flow flow)
        {
            switch (flow)
            {
                case OAuth2Flow.AuthorizationCode:
                    return Flows.AuthorizationCode;
                case OAuth2Flow.Implicit:
                    return Flows.Implicit;
                case OAuth2Flow.Hybrid:
                    return Flows.Hybrid;
                case OAuth2Flow.ClientCredentials:
                    return Flows.ClientCredentials;
                case OAuth2Flow.ResourceOwner:
                    return Flows.ResourceOwner;
                default:
                    return Flows.Implicit;
            }
        }

        private static List<string> SplitValues(string mergedValues)
        {
            if (mergedValues == null)
                throw new ArgumentNullException("mergedValues");

            return mergedValues
                .Split(' ', ',', ';', '\r', '\n', '\t')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();
        }
    }
}
