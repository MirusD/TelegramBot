using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using LinqToDB;
using static _102techBot.Program;

namespace _102techBot.Infrastructure.DAL
{
    internal class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _db;

        public OrderRepository(AppDbContext dbContext)
        {
            _db = dbContext;
        }
        public async Task<Order> AddAsync(Order newOrder)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var id = await _db.InsertWithInt64IdentityAsync(newOrder);
                    await transaction.CommitAsync();

                    newOrder.Id = id;
                    return newOrder;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при добавлении новой заявки на покупку ", ex.ToString());
                    throw;
                }
            }
        }

        public async Task<Order?> DeleteAsync(long orderId)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var existingOrder = await _db.GetTable<Order>()
                        .FirstOrDefaultAsync(o => o.Id == orderId);

                    if (existingOrder == null)
                    {
                        return null;
                    }

                    await _db.GetTable<Order>()
                        .DeleteAsync(r => r.Id == orderId);

                    await transaction.CommitAsync();
                    return existingOrder;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при удалении заявки на покупку " + ex.ToString());
                    throw;
                }
            }
        }

        public async Task<List<Order>> GetAllAsync()
        {
            try
            {
                return await _db.GetTable<Order>()
                    .LoadWith(r => r.User)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении всех заявок на покупку " + ex.ToString());
                throw;
            }
        }

        public async Task<Order?> GetByIdAsync(long orderId)
        {
            try
            {
                return await _db.GetTable<Order>()
                                    .FirstOrDefaultAsync(o => o.Id == orderId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении заявки на покупку по ID: {orderId} " + ex.ToString());
                throw;
            }
        }

        public async Task<List<Order>> GetByUserAsync(long userId)
        {
            try
            {
                return await _db.GetTable<Order>()
                    .Where(o => o.UserId == userId)
                    .LoadWith(r => r.User)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении заявок на покупку по пользователю {userId} " + ex.ToString());
                throw;
            }
        }

        public async Task<Order?> UpdateAsync(Order updatedOrder)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var existingOrder = await _db.GetTable<Order>()
                                                .FirstOrDefaultAsync(r => r.Id == updatedOrder.Id);

                    if (existingOrder == null)
                    {
                        return null;
                    }

                    await _db.UpdateAsync(updatedOrder);
                    await transaction.CommitAsync();
                    return updatedOrder;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при обновлении заявки на покуаку " + ex.ToString());
                    throw;
                }
            }
        }
    }
}
