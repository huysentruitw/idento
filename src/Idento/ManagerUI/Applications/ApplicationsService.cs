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
using AutoMapper;
using Idento.Core.Cryptography;
using Idento.Domain.Models;
using Idento.Domain.Stores;
using Idento.ManagerUI.Applications.Models;

namespace Idento.ManagerUI.Applications
{
    public class ApplicationsService
    {
        private IApplicationStore store;
        private IMapper mapper;

        public ApplicationsService(IApplicationStore store, IMapper mapper)
        {
            this.store = store;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<T>> GetAll<T>() where T : class
        {
            var entities = await store.GetAll();
            return mapper.Map<IEnumerable<T>>(entities.OrderBy(x => x.DisplayName));
        }

        public async Task<T> GetById<T>(Guid id) where T : class
        {
            var entity = await store.GetById(id);
            return entity != null ? mapper.Map<T>(entity) : null;
        }
        
        public async Task<EditOrCreate> Create()
        {
            var model = new EditOrCreate();
            mapper.Map(new Application(), model);
            model.Enabled = true;
            model.ClientId = Crypto.GenerateRandomToken(32);
            model.ClientSecret = Crypto.GenerateRandomToken(64);
            return await Task.FromResult(model);
        }

        public async Task Insert(EditOrCreate model)
        {
            var entity = new Application();
            mapper.Map(model, entity);
            await store.Create(entity);
        }

        public async Task<bool> Update(Guid id, EditOrCreate model)
        {
            var entity = await store.GetById(id);
            if (entity == null)
                return false;

            mapper.Map(model, entity);
            await store.Update(entity);
            return true;
        }

        public async Task Delete(Guid id)
        {
            await store.Delete(id);
        }
    }
}
