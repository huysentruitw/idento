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
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Idento.Core.IdentityServer.Stores
{
    internal class ResourceStore : IResourceStore
    {
//        private static readonly IEnumerable<Scope> scopes = new List<Scope>
//            {
//#region Identity scopes

//                StandardScopes.OpenId,
//                StandardScopes.ProfileAlwaysInclude,
//                StandardScopes.Email,
//                StandardScopes.Address,
//                StandardScopes.OfflineAccess,
//                StandardScopes.RolesAlwaysInclude,
//                StandardScopes.AllClaims,

//#endregion

//#region Resource scopes

//                new Scope
//                {
//                    Name = "id_api",
//                    DisplayName = "IdentityServer Web API",
//                    Type = ScopeType.Resource,
//                    Emphasize = true,
//                    ShowInDiscoveryDocument = false,
//                    Claims = new List<ScopeClaim>
//                    {
//                        new ScopeClaim(JwtClaimTypes.Name),
//                        new ScopeClaim(JwtClaimTypes.Role)
//                    }
//                },

//#endregion
//            };

//        public Task<IEnumerable<Scope>> FindScopesAsync(IEnumerable<string> scopeNames)
//        {
//            if (scopeNames == null) throw new ArgumentNullException(nameof(scopeNames));
//            var result = scopes.Where(s => scopeNames.Contains(s.Name));
//            return Task.FromResult(result);
//        }

//        public Task<IEnumerable<Scope>> GetScopesAsync(bool publicOnly = true)
//        {
//            return publicOnly
//                ? Task.FromResult(scopes.Where(s => s.ShowInDiscoveryDocument))
//                : Task.FromResult(scopes);
//        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<Resources> GetAllResources()
        {
            throw new NotImplementedException();
        }
    }
}