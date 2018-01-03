using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Idento.Domain.Entities;

namespace Idento.Domain.Stores
{
    public interface IUserApplicationStore
    {
        Task<UserApplications[]> GetAll();
        Task<UserApplications> FindById(Guid id);
        Task<Application[]> FindApplicationsByUserId(Guid userId);
        Task<Application[]> FindApplicationsByUserName(string userName);
        Task<UserApplications[]> FindUserApplicationsByUserId(Guid userId);
        Task<UserApplications> FindUserApplicationsByUserIdAndApplicationId(Guid userId, Guid applicationId);
        Task Create(User user, Application application);
        Task Update(Guid id, Action<UserApplications> updateAction);
        Task Delete(Guid id);
    }
}