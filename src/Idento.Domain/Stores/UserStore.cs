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
using System.Threading;
using System.Threading.Tasks;
using Idento.Domain.Exceptions;
using Idento.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Idento.Domain.Stores
{
    internal class UserStore : UserStore<User, Role, DataContext, Guid>, IUserStore
    {
        private readonly TenantContextResolver _tenantContext;

        public UserStore(DataContext dataContext, TenantContextResolver tenantContext)
            : base(dataContext)
        {
            if (tenantContext == null) throw new ArgumentNullException(nameof(tenantContext));
            _tenantContext = tenantContext;
        }

        public async Task<IList<User>> GetAll()
        {
            return await Context.Users.Where(x => x.TenantId == _tenantContext.TenantId).ToListAsync();
        }

        public override async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken = new CancellationToken())
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.TenantId = _tenantContext.TenantId;
            return await base.CreateAsync(user, cancellationToken);
        }

        public override async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken = new CancellationToken())
        {
            var entity = await FindByIdAsync(user.Id.ToString(), cancellationToken);
            if (entity == null) throw new ArgumentException("User does not exist");
            if (user.TenantId != entity.TenantId) throw new ArgumentException("Fiddling with TenantId is not supported");
            if (entity.TenantId != _tenantContext.TenantId) throw new TenantIdDoesNotMatchContextException(nameof(entity.TenantId));
            return await base.DeleteAsync(user, cancellationToken);
        }

        public override async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = new CancellationToken())
        {
            return await Context.Users.FirstOrDefaultAsync(
                x => x.Email.ToUpper().Equals(normalizedEmail.ToUpper()) && x.TenantId == _tenantContext.TenantId,
                cancellationToken);
        }

        public override Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken = new CancellationToken())
        {
            return base.FindByIdAsync(userId, cancellationToken);
        }

        public override Task<User> FindByLoginAsync(string loginProvider, string providerKey,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return base.FindByLoginAsync(loginProvider, providerKey, cancellationToken);
        }

        public override Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = new CancellationToken())
        {
            return base.FindByNameAsync(normalizedUserName, cancellationToken);
        }

        public override Task<IList<User>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken = new CancellationToken())
        {
            return base.GetUsersInRoleAsync(normalizedRoleName, cancellationToken);
        }

        public override Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken = new CancellationToken())
        {
            return base.UpdateAsync(user, cancellationToken);
        }

        public override IQueryable<User> Users { get; }
    }
}