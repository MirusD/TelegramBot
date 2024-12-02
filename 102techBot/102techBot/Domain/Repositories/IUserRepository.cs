using System.Linq.Expressions;
using _102techBot.Domain.Entities;

namespace _102techBot.Domain.Repositories
{
    internal interface IUserRepository
    {
        public Task<User> AddAsync(User user);
        public Task<User> DeleteAsync(long id);
        public Task<List<User>> GetAllAsync();
        public Task<List<User>> GetUsersByRoleAsync(Expression<Func<User, bool>> roleFilter);
        public Task<User?> GetByIdAsync(long id);
        public Task<User> UpdateAsync(User user);
    }
}
