using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using LinqToDB;
using static _102techBot.Program;

namespace _102techBot.Infrastructure.DAL
{
    internal class OrderItemRepository : IOrderItemRepository
    {
        private readonly AppDbContext _db;

        public OrderItemRepository(AppDbContext dbContext)
        {
            _db = dbContext;
        }
        public async Task<OrderItem> AddAsync(OrderItem newOrderItem)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var id = await _db.InsertWithInt64IdentityAsync(newOrderItem);
                    await transaction.CommitAsync();

                    newOrderItem.Id = id;
                    return newOrderItem;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при добавлении нового товара в заявку на покупку ", ex.ToString());
                    throw;
                }
            }
        }

        public async Task<OrderItem?> DeleteAsync(long orderItemId)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var existingOrderItem = await _db.GetTable<OrderItem>()
                        .FirstOrDefaultAsync(oi => oi.Id == orderItemId);

                    if (existingOrderItem == null)
                    {
                        return null;
                    }

                    await _db.GetTable<Order>()
                        .DeleteAsync(oi => oi.Id == orderItemId);

                    await transaction.CommitAsync();
                    return existingOrderItem;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при удалении товара из заявки на покупку " + ex.ToString());
                    throw;
                }
            }
        }

        public async Task<List<OrderItem>> GetAllAsync()
        {
            try
            {
                return await _db.GetTable<OrderItem>()
                    .LoadWith(oi => oi.Product)
                    .LoadWith(oi => oi.Order)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении всех товаров заказанных на покупку " + ex.ToString());
                throw;
            }
        }

        public async Task<OrderItem?> GetByIdAsync(long orderItemId)
        {
            try
            {
                return await _db.GetTable<OrderItem>()
                                    .FirstOrDefaultAsync(o => o.Id == orderItemId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении товара из заявки на покупку по ID: {orderItemId} " + ex.ToString());
                throw;
            }
        }

        public async Task<List<OrderItem>> GetByOrder(long orderId)
        {
            try
            {
                return await _db.GetTable<OrderItem>()
                    .Where(oi => oi.OrderId == orderId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении товаров из заявки на покупку {orderId} " + ex.ToString());
                throw;
            }
        }

        public async Task<OrderItem?> UpdateAsync(OrderItem updatedOrderItem)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var existingOrderItem = await _db.GetTable<OrderItem>()
                                                .FirstOrDefaultAsync(oi => oi.Id == updatedOrderItem.Id);

                    if (existingOrderItem == null)
                    {
                        return null;
                    }

                    await _db.UpdateAsync(updatedOrderItem);
                    await transaction.CommitAsync();
                    return updatedOrderItem;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при обновлении товара который заказан на покупку " + ex.ToString());
                    throw;
                }
            }
        }
    }
}
