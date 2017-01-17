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
using Idento.Domain.Exceptions;
using Idento.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Idento.Domain.Stores
{
    internal class ApplicationStore : IApplicationStore
    {
        private readonly DataContext _dataContext;
        private readonly TenantContext _tenantContext;

        public ApplicationStore(DataContext dataContext, TenantContext tenantContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            if (tenantContext == null) throw new ArgumentNullException(nameof(tenantContext));
            _dataContext = dataContext;
            _tenantContext = tenantContext;
        }

        public async Task<Application> GetById(Guid id)
        {
            return await _dataContext.Applications.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenantContext.TenantId);
        }

        public async Task<Application> GetByClientId(string clientId)
        {
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException(nameof(clientId));
            return await _dataContext.Applications.FirstOrDefaultAsync(x => x.ClientId == clientId && x.TenantId == _tenantContext.TenantId);
        }

        public async Task<Application> GetByDisplayName(string displayName)
        {
            if (string.IsNullOrEmpty(displayName)) throw new ArgumentNullException(nameof(displayName));
            return await _dataContext.Applications.FirstOrDefaultAsync(x => x.DisplayName == displayName && x.TenantId == _tenantContext.TenantId);
        }

        public async Task<IList<Application>> GetAll()
        {
            return await _dataContext.Applications.Where(x => x.TenantId == _tenantContext.TenantId).ToListAsync();
        }

        public async Task<int> Create(Application entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (entity.TenantId != _tenantContext.TenantId) throw new TenantIdDoesNotMatchContextException(nameof(entity.TenantId));
            _dataContext.Applications.Add(entity);
            return await _dataContext.SaveChangesAsync();
        }

        public async Task<int> Update(Application entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (entity.TenantId != _tenantContext.TenantId) throw new TenantIdDoesNotMatchContextException(nameof(entity.TenantId));
            return await _dataContext.SaveChangesAsync();
        }

        public async Task<Application> Delete(Guid id)
        {
            var entity = await GetById(id);
            if (entity != null)
            {
                if (entity.TenantId != _tenantContext.TenantId) throw new TenantIdDoesNotMatchContextException(nameof(entity.TenantId));
                _dataContext.Applications.Remove(entity);
                await _dataContext.SaveChangesAsync();
            }
            return entity;
        }
    }
}