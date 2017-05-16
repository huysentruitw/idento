using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Idento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Idento.Domain.Stores
{
    public class UserApplicationsStore : IUserApplicationStore
    {
        private readonly DataContext _db;

        public UserApplicationsStore(DataContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public Task<UserApplications[]> GetAll()
            => _db.UserApplications.AsNoTracking().OrderBy(x => x.User.UserName).ThenBy(x => x.Application.Name)
                .ToArrayAsync();

        public Task<UserApplications> FindById(Guid id)
            => _db.UserApplications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public Task<Application[]> FindApplicationsByUserId(Guid userId)
            => _db.Applications.AsNoTracking().Where(x => x.Users.Any(u => u.UserId == userId)).ToArrayAsync();

        public Task<Application[]> FindApplicationsByUserName(string userName)
        {
            throw new NotImplementedException();
        }

        public Task<UserApplications[]> FindUserApplicationsByUserId(Guid userId)
            => _db.UserApplications.AsNoTracking().Where(x => x.UserId == userId).ToArrayAsync();

        public Task<UserApplications> FindUserApplicationsByUserIdAndApplicationId(Guid userId, Guid applicationId)
            => _db.UserApplications.AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ApplicationId == applicationId);

        public Task<UserApplications[]> FindUserApplicationsByUserName(string userName)
        {
            throw new NotImplementedException();
        }

        //todo: do we need to have a createDate here also?
        public Task Create(User user, Application application)
        {
            var userApplications = new UserApplications
            {
                //User = user,
                //Application = application,
                UserId = user.Id,
                ApplicationId = application.Id
            };

            _db.UserApplications.Add(userApplications);

            return _db.SaveChangesAsync();
        }

        //todo: do we need to update the user it's DateUpdated property?
        public Task Update(Guid id, Action<UserApplications> updateAction)
        {
            var userApplications = new UserApplications {Id = id};
            _db.UserApplications.Attach(userApplications);
            updateAction(userApplications);
            return _db.SaveChangesAsync();
        }

        public Task Delete(Guid id)
        {
            var userApplications = new UserApplications {Id = id};
            _db.UserApplications.Remove(userApplications);
            return _db.SaveChangesAsync();
        }
    }
}