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
using Idento.Domain.Models;
using Microsoft.Data.Entity;

namespace Idento.Domain.Stores
{
    internal class ExternalLoginProviderStore : IExternalLoginProviderStore
    {
        private DataContext dataContext;

        public ExternalLoginProviderStore(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<ExternalLoginProvider> GetById(Guid id)
        {
            return await dataContext.ExternalLoginProviders.SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IList<ExternalLoginProvider>> GetAll()
        {
            return await dataContext.ExternalLoginProviders.ToListAsync();
        }

        public async Task<IList<ExternalLoginProvider>> GetEnabled()
        {
            return await dataContext.ExternalLoginProviders.Where(x => x.Enabled).ToListAsync();
        }

        public async Task<int> Create(ExternalLoginProvider entity)
        {
            dataContext.ExternalLoginProviders.Add(entity);
            return await dataContext.SaveChangesAsync();
        }

        public async Task<int> Update(ExternalLoginProvider entity)
        {
            return await dataContext.SaveChangesAsync();
        }

        public async Task<ExternalLoginProvider> Delete(Guid id)
        {
            var entity = await GetById(id);
            if (entity != null)
            {
                dataContext.ExternalLoginProviders.Remove(entity);
                await dataContext.SaveChangesAsync();
            }
            return entity;
        }
    }
}
