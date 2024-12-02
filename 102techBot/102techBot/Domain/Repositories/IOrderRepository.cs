using _102techBot.Domain.Entities;

namespace _102techBot.Domain.Repositories
{

    internal interface IOrderRepository
    {
        public Task<Order> AddAsync(Order newOrder);
        public Task<Order?> DeleteAsync(long orderId);
        public Task<List<Order>> GetAllAsync();
        public Task<Order?> GetByIdAsync(long orderId);
        public Task<List<Order>> GetByUserAsync(long userId);
        public Task<Order?> UpdateAsync(Order updatedOrder);
    }
}
