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
using System.Linq;
using System.Threading.Tasks;
using Idento.Domain.Models;

namespace Idento.Domain.Repositories
{
    public class ExternalLoginProviderRepository : Repository
    {
        public ExternalLoginProviderRepository(DataContext ctx)
            : base(ctx)
        {
        }

        public async Task<ExternalLoginProvider> GetById(Guid id)
        {
            return await ctx.ExternalLoginProviders.SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IList<ExternalLoginProvider>> GetAll()
        {
            return await ctx.ExternalLoginProviders.ToListAsync();
        }

        public async Task<IList<ExternalLoginProvider>> GetEnabled()
        {
            return await ctx.ExternalLoginProviders.Where(x => x.Enabled).ToListAsync();
        }

        public async Task<ExternalLoginProvider> Create()
        {
            return await Task.FromResult(ctx.ExternalLoginProviders.Create());
        }

        public async Task<Guid> Insert(ExternalLoginProvider entity)
        {
            ctx.ExternalLoginProviders.Add(entity);
            await ctx.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> Update(ExternalLoginProvider entity)
        {
            return await ctx.SaveChangesAsync();
        }

        public async Task<ExternalLoginProvider> Delete(Guid id)
        {
            var entity = await GetById(id);
            if (entity != null)
            {
                ctx.ExternalLoginProviders.Remove(entity);
                await ctx.SaveChangesAsync();
            }
            return entity;
        }
    }
}
