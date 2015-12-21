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

using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using Idento.Domain.Models;
using Idento.Domain.Repositories;

namespace Idento.Core.IdentityServer3.Implementations
{
    internal class ClientStore : IClientStore
    {
        private ApplicationRepository repository;

        public ClientStore(ApplicationRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var application = await this.repository.GetByClientId(clientId);
            if (application == null)
                return null;

            return new Client
            {
                ClientId = application.ClientId,
                ClientSecrets = new List<Secret> { new Secret(application.ClientSecret) },
                ClientName = application.DisplayName,
                Enabled = application.Enabled,
                ClientUri = application.Uri,
                RedirectUris = new List<string> { application.RedirectUri },
                PostLogoutRedirectUris = new List<string> { application.RedirectUri },
                AccessTokenLifetime = application.TokenLifetimeInMinutes * 60,
                IdentityTokenLifetime = application.TokenLifetimeInMinutes * 60,
                Flow = Translate(application.Flow),
                RequireConsent = application.RequireConsent,
                AllowRememberConsent = true
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
    }
}
