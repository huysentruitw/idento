using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Idento.Domain.Models;

namespace Idento.Domain.Stores
{
    public interface ITenantStore
    {
        Task<Tenant> GetById(Guid id);
        Task<IList<Tenant>> GetAll();
        Task<int> Create(Tenant entity);
        Task<int> Update(Tenant entity);
        Task<Tenant> Delete(Guid id);
    }
}
