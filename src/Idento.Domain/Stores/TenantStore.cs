using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Idento.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Idento.Domain.Stores
{
    internal class TenantStore : ITenantStore
    {
        private readonly DataContext _dataContext;

        public TenantStore(DataContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            _dataContext = dataContext;
        }

        public async Task<Tenant> GetById(Guid id)
        {
            return await _dataContext.Tenants.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IList<Tenant>> GetAll()
        {
            return await _dataContext.Tenants.ToListAsync();
        }

        public async Task<int> Create(Tenant entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _dataContext.Tenants.Add(entity);
            return await _dataContext.SaveChangesAsync();
        }

        public async Task<int> Update(Tenant entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            return await _dataContext.SaveChangesAsync();
        }

        public async Task<Tenant> Delete(Guid id)
        {
            var entity = await GetById(id);
            if (entity != null)
            {
                _dataContext.Tenants.Remove(entity);
                await _dataContext.SaveChangesAsync();
            }
            return entity;
        }
    }
}
