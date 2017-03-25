/*
 * Copyright 2016-2017 Wouter Huysentruit
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
using System.Threading.Tasks;
using Idento.Domain.Entities;

namespace Idento.Domain.Stores
{
    internal class TenantStore : ITenantStore
    {
        private readonly DataContext _db;

        public TenantStore(DataContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task Create(Tenant tenant)
        {
            tenant.DateCreated = DateTime.Now;
            _db.Tenants.Add(tenant);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var tenant = new Tenant { Id = id };
            _db.Tenants.Remove(tenant);
            await _db.SaveChangesAsync();
        }

        public Task<Tenant> FindById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Tenant[]> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task Update(Tenant tenant)
        {
            tenant.DateUpdated = DateTime.Now;
            _db.Tenants.Update(tenant);
            await _db.SaveChangesAsync();
        }
    }
}
