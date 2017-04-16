﻿/*
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
using System.Linq;
using System.Threading.Tasks;
using Idento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Idento.Domain.Stores
{
    internal class CertificateStore : ICertificateStore
    {
        private readonly DataContext _db;

        public CertificateStore(DataContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public Task Create(Guid tenantId, Certificate certificate)
        {
            certificate.TenantId = tenantId;
            certificate.DateCreated = DateTime.Now;
            _db.Certificates.Add(certificate);
            return _db.SaveChangesAsync();
        }

        public Task Delete(Guid tenantId, Guid id)
        {
            var certificate = new Certificate { Id = id, TenantId = tenantId };
            _db.Certificates.Remove(certificate);
            return _db.SaveChangesAsync();
        }

        public Task<Certificate> FindById(Guid tenantId, Guid id)
            => _db.Certificates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);

        public Task<Certificate> FindByName(Guid tenantId, string name)
            => _db.Certificates.AsNoTracking().FirstOrDefaultAsync(x => string.Compare(x.Name, name, true) == 0 && x.TenantId == tenantId);

        public Task<Certificate[]> GetAll(Guid tenantId)
            => _db.Certificates.AsNoTracking().Where(x => x.TenantId == tenantId).OrderBy(x => x.Name).ToArrayAsync();

        public Task Update(Guid tenantId, Guid id, Action<Certificate> updateAction)
        {
            var certificate = new Certificate { Id = id, TenantId = tenantId };
            _db.Certificates.Attach(certificate);
            updateAction(certificate);
            certificate.DateUpdated = DateTime.Now;
            return _db.SaveChangesAsync();
        }
    }
}
