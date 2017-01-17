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
using Idento.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Idento.Domain.Stores
{
    internal class CertificateStore : ICertificateStore
    {
        private readonly DataContext _dataContext;
        private readonly TenantContext _tenantContext;

        public CertificateStore(DataContext dataContext, TenantContext tenantContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            if (tenantContext == null) throw new ArgumentNullException(nameof(tenantContext));
            _dataContext = dataContext;
            _tenantContext = tenantContext;
        }

        public async Task<Certificate> GetById(Guid id)
        {
            return await _dataContext.Certificates.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenantContext.TenantId);
        }

        public async Task<IList<Certificate>> GetByPurpose(CertificatePurpose purpose)
        {
            return await _dataContext.Certificates.Where(x => x.Purpose == purpose && x.TenantId == _tenantContext.TenantId).ToListAsync();
        }

        public async Task<Certificate> GetFirstByPurpose(CertificatePurpose purpose)
        {
            return await _dataContext.Certificates.FirstOrDefaultAsync(x => x.Purpose == purpose && x.TenantId == _tenantContext.TenantId);
        }
    }
}
