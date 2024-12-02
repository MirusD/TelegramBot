using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using LinqToDB;
using static _102techBot.Program;

namespace _102techBot.Infrastructure.DAL
{
    internal class CartItemRepository : ICartItemRepository
    {
        private readonly AppDbContext _db;

        public CartItemRepository(AppDbContext dbContext)
        {
            _db = dbContext;
        }
        public async Task<CartItem> AddAsync(CartItem newCartItem)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var id = await _db.InsertWithInt64IdentityAsync(newCartItem);
                    await transaction.CommitAsync();

                    newCartItem.Id = id;
                    return newCartItem;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при добавлении товара в корзину", ex.ToString());   
                    throw;
                }
            }
        }

        public async Task<CartItem?> DeleteAsync(long cartItemId)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var existingCartItem = await _db.GetTable<CartItem>()
                                                .FirstOrDefaultAsync(c => c.Id == cartItemId);

                    if (existingCartItem == null)
                    {
                        return null;
                    }

                    await _db.GetTable<CartItem>()
                              .Where(c => c.Id == cartItemId)
                              .DeleteAsync();

                    await transaction.CommitAsync();
                    return existingCartItem;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при удалении товара из корзины" + ex.ToString());
                    throw;
                }
            }
        }

        public async Task<int> DeleteAllByCart(long cartId)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var affectedRows = await _db.GetTable<CartItem>()
                        .Where(ci => ci.CartId == cartId)
                        .DeleteAsync();

                    await transaction.CommitAsync();

                    return affectedRows;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Ошибка при удалении CartItems для CartId {cartId} : {ex}");
                    throw;
                }
            }
        }

        public async Task<List<CartItem>> GetAllAsync()
        {
            try
            {
                return await _db.GetTable<CartItem>().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении всех товаров из корзины" + ex.ToString());
                throw;
            }
        }

        public async Task<List<CartItem>> GetByCartAsync(long cartId)
        {
            try
            {
                return await _db.GetTable<CartItem>()
                                .Where(ci => ci.CartId == cartId)
                                .LoadWith(r => r.Product)
                                .LoadWith(r => r.Cart)
                                .ToListAsync();;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении товаров корзины с ID {cartId}" + ex.ToString());
                throw; // Пробрасываем исключение для обработки на более высоком уровне
            }
        }

        public async Task<CartItem?> GetByIdAsync(long cartItemId)
        {
            try
            {
                return await _db.GetTable<CartItem>()
                                    .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка получения товара по id" + ex.ToString());
                throw;
            }
        }

        public async Task<CartItem?> UpdateAsync(CartItem updatedCartItem)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var existingCartItem = await _db.GetTable<CartItem>()
                                                .FirstOrDefaultAsync(c => c.Id == updatedCartItem.Id);

                    if (existingCartItem == null)
                    {
                        return null;
                    }

                    await _db.UpdateAsync(updatedCartItem);
                    await transaction.CommitAsync();
                    return updatedCartItem;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при обновлении товара в корзине" + ex.ToString());
                    throw;
                }
            }
        }
    }
}
