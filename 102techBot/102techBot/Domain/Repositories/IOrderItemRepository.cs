using _102techBot.Domain.Entities;

namespace _102techBot.Domain.Repositories
{
    internal interface IOrderItemRepository
    {
        public Task<OrderItem> AddAsync(OrderItem newOrderItem);
        public Task<OrderItem?> DeleteAsync(long orderItemId);
        public Task<List<OrderItem>> GetAllAsync();
        public Task<OrderItem?> GetByIdAsync(long orderItemId);
        public Task<List<OrderItem>> GetByOrder(long userId);
        public Task<OrderItem?> UpdateAsync(OrderItem updatedOrderItem);
    }
}
