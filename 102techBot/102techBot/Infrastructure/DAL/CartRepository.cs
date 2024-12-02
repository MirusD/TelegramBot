using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using LinqToDB;
using static _102techBot.Program;

namespace _102techBot.Infrastructure.DAL
{
    internal class CartRepository : ICartRepository
    {
        private readonly AppDbContext _db;

        public CartRepository(AppDbContext dbContext)
        {
            _db = dbContext;
        }

        public async Task<Cart> AddAsync(Cart newCart)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var id = await _db.InsertWithInt64IdentityAsync(newCart);
                    await transaction.CommitAsync();

                    newCart.Id = id;
                    return newCart;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при добавлении корзины" + ex.ToString());
                    throw;
                }
            }
        }

        public async Task<Cart?> DeleteAsync(long cartId)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var existingCart = await _db.GetTable<Cart>()
                        .FirstOrDefaultAsync(c => c.Id == cartId);

                    if (existingCart == null)
                    {
                        return null;
                    }

                    await _db.GetTable<Cart>()
                              .DeleteAsync(c => c.Id == cartId);

                    await transaction.CommitAsync();
                    return existingCart;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при удалении корзины" + ex.ToString());
                    throw;
                }
            }
        }

        public async Task<List<Cart>> GetAllAsync()
        {
            try
            {
                var carts = await _db.GetTable<Cart>().ToListAsync();
                return carts;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении всех корзин корзины" + ex.ToString());
                throw;
            }
        }

        public async Task<Cart?> GetByIdAsync(long cartId)
        {
            try
            {
                return await _db.GetTable<Cart>()
                                    .FirstOrDefaultAsync(c => c.Id == cartId);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении корзины по ID: {cartId}" + ex.ToString());
                throw;
            }
        }

        public async Task<Cart?> GetByUserAsync(long userId)
        {
            try
            {
                return await _db.GetTable<Cart>()
                    .FirstOrDefaultAsync(c => c.UserId == userId);;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении корзины по пользователю" + ex.ToString());
                throw;
            }
        }

        public async Task<Cart?> UpdateAsync(Cart updatedCart)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var existingCart = await _db.GetTable<Cart>()
                                                .FirstOrDefaultAsync(c => c.Id == updatedCart.Id);

                    if (existingCart == null)
                    {
                        return null;
                    }

                    await _db.UpdateAsync(updatedCart);
                    await transaction.CommitAsync();
                    return updatedCart;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при обновлении корзины" + ex.ToString());
                    throw;
                }
            }
        }
    }
}
