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
using Idento.Web.ManagerUI.Users.Models;
using Microsoft.AspNetCore.Identity;

namespace Idento.Web.ManagerUI.Users
{
    public class UsersService
    {
        private readonly IUserStore _store;
        private readonly UserManager<User> _manager;
        private readonly IMapper _mapper;

        public UsersService(IUserStore store, UserManager<User> manager, IMapper mapper)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));
            _store = store;
            _manager = manager;
            _mapper = mapper;
        }

        public async Task<IEnumerable<T>> GetAll<T>() where T : class
        {
            var entities = await _store.GetAll();
            return _mapper.Map<IEnumerable<T>>(entities.OrderBy(x => x.Email));
        }

        public async Task<T> GetById<T>(Guid id) where T : class
        {
            var entity = await _manager.FindByIdAsync(id.ToString());
            return entity != null ? _mapper.Map<T>(entity) : null;
        }

        public async Task<Create> Create()
        {
            var model = new Create();
            _mapper.Map(new User(), model);
            return await Task.FromResult(model);
        }

        public async Task<IdentityResult> Insert(Create model)
        {
            var entity = new User();
            _mapper.Map(model, entity);
            return await _manager.CreateAsync(entity, model.Password);
        }

        public async Task<bool> Update(Guid id, Create model)
        {
            var entity = await _manager.FindByIdAsync(id.ToString());
            if (entity == null)
                return false;

            _mapper.Map(model, entity);
            await _manager.UpdateAsync(entity);
            return true;
        }

        public async Task Delete(Guid id)
        {
            var entity = await _manager.FindByIdAsync(id.ToString());
            if (entity != null)
                await _manager.DeleteAsync(entity);
        }

        public async Task<bool> EmailInUse(string email)
        {
            var user = await _manager.FindByEmailAsync(email);
            return user != null;
        }
    }
}
