/*
 * Copyright 2016-2018 Wouter Huysentruit
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
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Idento.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Idento.Domain.Stores
{
    internal class UserStore : UserStore<User, Role, DataContext, Guid>
    {
        private readonly DataContext _dataContext;

        public UserStore(DataContext dataContext)
            : base(dataContext)
        {
            _dataContext = dataContext;
        }

        public new void Dispose()
        {
            _dataContext.Dispose();
        }

        public Task<Guid> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public override Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public override Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            _dataContext.Users.Attach(user);
            _dataContext.SaveChanges();

            return Task.FromResult(user.UserName);
        }

        public override Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public override Task SetNormalizedUserNameAsync(User user, string normalizedName,
            CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            _dataContext.Users.Attach(user);
            _dataContext.SaveChanges();
            return Task.FromResult(user.NormalizedUserName);
        }

        public override Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            _dataContext.Users.Add(user);
            _dataContext.SaveChanges();
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            _dataContext.Users.Attach(user);
            _dataContext.SaveChanges();
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            _dataContext.Users.Remove(user);
            _dataContext.SaveChanges();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<User> FindByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = _dataContext.Users.FirstOrDefault(u => u.Id == userId);

            return Task.FromResult(user);
        }

        public override Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            if (normalizedUserName != null)
            {
                var user = _dataContext.Users.FirstOrDefault(u => u.NormalizedUserName == normalizedUserName);
                return Task.FromResult(user);
            }

            return Task.FromResult(new User());
        }
    }
}