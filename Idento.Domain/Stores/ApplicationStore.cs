/*
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
    internal class ApplicationStore : IApplicationStore
    {
        private readonly DataContext _db;

        public ApplicationStore(DataContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public Task Create(Application application)
        {
            application.DateCreated = DateTime.Now;
            _db.Applications.Add(application);
            return _db.SaveChangesAsync();
        }

        public Task Delete(Guid id)
        {
            var application = new Application { Id = id };
            _db.Applications.Remove(application);
            return _db.SaveChangesAsync();
        }

        public Task<Application> FindById(Guid id)
            => _db.Applications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public Task<Application> FindByName(string name)
            => _db.Applications.AsNoTracking().FirstOrDefaultAsync(x => string.Compare(x.Name, name, true) == 0);

        public Task<Application[]> GetAll()
            => _db.Applications.AsNoTracking().OrderBy(x => x.Name).ToArrayAsync();

        public Task Update(Guid id, Action<Application> updateAction)
        {
            var application = new Application { Id = id };
            _db.Applications.Attach(application);
            updateAction(application);
            application.DateUpdated = DateTime.Now;
            return _db.SaveChangesAsync();
        }
    }
}
