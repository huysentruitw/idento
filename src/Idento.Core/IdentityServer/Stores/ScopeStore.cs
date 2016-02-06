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
using IdentityServer4.Core;
using IdentityServer4.Core.Models;
using IdentityServer4.Core.Services;

namespace Idento.Core.IdentityServer.Stores
{
    internal class ScopeStore : IScopeStore
    {
        private static readonly IEnumerable<Scope> scopes = new List<Scope>
            {
#region Identity scopes

                StandardScopes.OpenId,
                StandardScopes.ProfileAlwaysInclude,
                StandardScopes.Email,
                StandardScopes.Address,
                StandardScopes.OfflineAccess,
                StandardScopes.RolesAlwaysInclude,
                StandardScopes.AllClaims,

#endregion

#region Resource scopes

                new Scope
                {
                    Name = "id_api",
                    DisplayName = "IdentityServer Web API",
                    Type = ScopeType.Resource,
                    Emphasize = true,
                    ShowInDiscoveryDocument = false,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim(Constants.ClaimTypes.Name),
                        new ScopeClaim(Constants.ClaimTypes.Role)
                    }
                },

#endregion
            };

        public Task<IEnumerable<Scope>> FindScopesAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null) throw new ArgumentNullException(nameof(scopeNames));
            var result = scopes.Where(s => scopeNames.Contains(s.Name));
            return Task.FromResult<IEnumerable<Scope>>(result.ToList());
        }

        public Task<IEnumerable<Scope>> GetScopesAsync(bool publicOnly = true)
        {
            return publicOnly
                ? Task.FromResult(scopes.Where(s => s.ShowInDiscoveryDocument))
                : Task.FromResult(scopes);
        }
    }
}
