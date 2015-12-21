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
using System.Data.Entity;
using System.Threading.Tasks;
using Idento.Domain.Models;

namespace Idento.Domain.Repositories
{
    public class ApplicationRepository : Repository
    {
        public ApplicationRepository(DataContext ctx)
            : base(ctx)
        {
        }

        public async Task<Application> GetById(Guid id)
        {
            return await ctx.Applications.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Application> GetByClientId(string clientId)
        {
            return await ctx.Applications.FirstOrDefaultAsync(x => x.ClientId == clientId);
        }

        public async Task<IList<Application>> GetAll()
        {
            return await ctx.Applications.ToListAsync();
        }

        public async Task<Application> Create()
        {
            return await Task.FromResult(ctx.Applications.Create());
        }

        public async Task<Guid> Insert(Application entity)
        {
            ctx.Applications.Add(entity);
            await ctx.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> Update(Application entity)
        {
            return await ctx.SaveChangesAsync();
        }

        public async Task<Application> Delete(Guid id)
        {
            var entity = await GetById(id);
            if (entity != null)
            {
                ctx.Applications.Remove(entity);
                await ctx.SaveChangesAsync();
            }
            return entity;
        }
    }
}
