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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Idento.Domain.Models;
using Idento.Domain.Stores;

namespace Idento.Core.IdentityServer.Stores
{
    internal class ClientStore : IClientStore
    {
        private readonly IApplicationStore store;

        public ClientStore(IApplicationStore store)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            this.store = store;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            if (clientId == null) throw new ArgumentNullException(nameof(clientId));

            var application = await this.store.GetByClientId(clientId);
            if (application == null || !application.Enabled)
                return null;

            var redirectUris = SplitValues(application.RedirectUris);
            var allowedScopes = SplitValues(application.AllowedScopes ?? string.Empty);
            var allowedExternalLoginProviders = SplitValues(application.AllowedExternalLoginProviders ?? string.Empty);

            var allowedCorsOrigins = SplitValues(application.AllowedCorsOrigins ?? string.Empty);
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

                AllowedGrantTypes = Translate(application.GrantType),

                RequireConsent = application.RequireConsent,
                AllowRememberConsent = true,

                AllowedScopes = allowedScopes,

                AllowedCorsOrigins = allowedCorsOrigins,
                IdentityProviderRestrictions = allowedExternalLoginProviders,
            };
        }

        /// <summary>
        /// Translate CogniStreamer IdentityServer OAuth2GrantType to ThinkTecture IdentityServer4 grant type IEnumerable.
        /// </summary>
        /// <param name="grantType">The CongniStreamer IdentityServer <see cref="OAuth2GrantType"/> enumeration.</param>
        /// <returns>The ThinkTecture IdentityServer3 variant.</returns>
        private static IEnumerable<string> Translate(OAuth2GrantType grantType)
        {
            switch (grantType)
            {
                case OAuth2GrantType.Implicit:
                    return GrantTypes.Implicit;
                case OAuth2GrantType.ImplicitAndClientCredentials:
                    return GrantTypes.ImplicitAndClientCredentials;
                case OAuth2GrantType.Code:
                    return GrantTypes.Code;
                case OAuth2GrantType.CodeAndClientCredentials:
                    return GrantTypes.CodeAndClientCredentials;
                case OAuth2GrantType.Hybrid:
                    return GrantTypes.Hybrid;
                case OAuth2GrantType.HybridAndClientCredentials:
                    return GrantTypes.HybridAndClientCredentials;
                case OAuth2GrantType.ClientCredentials:
                    return GrantTypes.ClientCredentials;
                case OAuth2GrantType.ResourceOwnerPassword:
                    return GrantTypes.ResourceOwnerPassword;
                case OAuth2GrantType.ResourceOwnerPasswordAndClientCredentials:
                    return GrantTypes.ResourceOwnerPasswordAndClientCredentials;
                default:
                    return Enumerable.Empty<string>();
            }
        }

        private static List<string> SplitValues(string mergedValues)
        {
            if (mergedValues == null) throw new ArgumentNullException(nameof(mergedValues));

            return mergedValues
                .Split(' ', ',', ';', '\r', '\n', '\t')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();
        }
    }
}