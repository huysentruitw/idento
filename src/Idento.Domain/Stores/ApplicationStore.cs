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

using System.Threading.Tasks;
using Idento.Domain.Models;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;

namespace Idento.Domain.Stores
{
    internal class ApplicationStore : IApplicationStore
    {
        private DataContext dataContext;

        public ApplicationStore(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<Application> GetById(Guid id)
        {
            return await dataContext.Applications.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Application> GetByClientId(string clientId)
        {
            return await dataContext.Applications.FirstOrDefaultAsync(x => x.ClientId == clientId);
        }

        public async Task<Application> GetByDisplayName(string displayName)
        {
            return await dataContext.Applications.FirstOrDefaultAsync(x => x.DisplayName == displayName);
        }

        public async Task<IList<Application>> GetAll()
        {
            return await dataContext.Applications.ToListAsync();
        }

        public async Task<int> Create(Application entity)
        {
            dataContext.Applications.Add(entity);
            return await dataContext.SaveChangesAsync();
        }

        public async Task<int> Update(Application entity)
        {
            return await dataContext.SaveChangesAsync();
        }

        public async Task<Application> Delete(Guid id)
        {
            var entity = await GetById(id);
            if (entity != null)
            {
                dataContext.Applications.Remove(entity);
                await dataContext.SaveChangesAsync();
            }
            return entity;
        }
    }
}
