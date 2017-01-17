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
using AutoMapper;
using Idento.Domain.Models;
using Idento.Domain.Stores;
using Idento.Web.ManagerUI.LoginProviders.Models;

namespace Idento.Web.ManagerUI.LoginProviders
{
    public class LoginProvidersService
    {
        private readonly IExternalLoginProviderStore _store;
        private readonly IMapper _mapper;

        public LoginProvidersService(IExternalLoginProviderStore store, IMapper mapper)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));
            _store = store;
            _mapper = mapper;
        }

        public async Task<IEnumerable<T>> GetAll<T>() where T : class
        {
            var entities = await _store.GetAll();
            return _mapper.Map<IEnumerable<T>>(entities.OrderBy(x => x.Name));
        }

        public async Task<T> GetById<T>(Guid id) where T : class
        {
            var entity = await _store.GetById(id);
            return entity != null ? _mapper.Map<T>(entity) : null;
        }

        public async Task<EditOrCreate> Create()
        {
            var model = new EditOrCreate();
            _mapper.Map(new ExternalLoginProvider(), model);
            model.Enabled = true;
            return await Task.FromResult(model);
        }

        public async Task Insert(EditOrCreate model)
        {
            var entity = new ExternalLoginProvider();
            _mapper.Map(model, entity);
            await _store.Create(entity);
        }

        public async Task<bool> Update(Guid id, EditOrCreate model)
        {
            var entity = await _store.GetById(id);
            if (entity == null)
                return false;

            _mapper.Map(model, entity);
            await _store.Update(entity);
            return true;
        }

        public async Task Delete(Guid id)
        {
            await _store.Delete(id);
        }
    }
}
