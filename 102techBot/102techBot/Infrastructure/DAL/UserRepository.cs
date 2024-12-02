using _102techBot.Domain.Repositories;
using LinqToDB;
using System.Linq.Expressions;
using static _102techBot.Program;
using User = _102techBot.Domain.Entities.User;

namespace _102techBot.Infrastructure.DAL
{
    internal class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext dbContext) 
        {
            _db = dbContext;
        }
        public async Task<User> AddAsync(User user)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    await _db.InsertAsync(user);
                    await transaction.CommitAsync();
                    return user;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при добавлении нового пользователя: " + ex.ToString());
                    throw;
                }
            }
        }

        public async Task<List<User>> GetUsersByRoleAsync(Expression<Func<User, bool>> roleFilter)
        {
            try
            {
                return await _db.Users
                    .Where(roleFilter)
                    .ToListAsync();
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Ошибка при получении пользователя по его роли: " + ex.ToString());
                throw;
            }
        }

        public async Task<User?> DeleteAsync(long userId)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var existingUser = await _db.GetTable<User>()
                                                .FirstOrDefaultAsync(u => u.Id == userId);

                    if (existingUser == null)
                    {
                        return null;
                    }

                    await _db.GetTable<User>()
                              .DeleteAsync(u => u.Id == userId);

                    await transaction.CommitAsync();
                    return existingUser;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при удалении пользователя" + ex.ToString());
                    throw;
                }
            }
        }

        public async Task<List<User>> GetAllAsync()
        {
            try
            {
                return await _db.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении всех пользователей: " + ex.ToString());
                throw;
            }
        }

        public async Task<User?> GetByIdAsync(long id)
        {
            try
            {
                return await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (LinqToDBException dbEx)
            {
                Console.WriteLine($"Ошибка базы данных: {dbEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении пользователя: " + ex.ToString());
                throw;
            }
        }
        public async Task<User> UpdateAsync(User user)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var existingUser = await _db.GetTable<User>()
                            .FirstOrDefaultAsync(c => c.Id == user.Id);

                    await _db.UpdateAsync(user);
                    await transaction.CommitAsync();
                    return user;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при обновлении пользователя: " + ex.ToString());
                    throw;
                }
            }
        }
    }
}
