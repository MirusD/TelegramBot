using _102techBot.Domain.Entities;

namespace _102techBot.Domain.Repositories
{
    internal interface ICartRepository
    {
        public Task<Cart> AddAsync(Cart cart);
        public Task<Cart?> DeleteAsync(long id);

        public Task<List<Cart>> GetAllAsync();

        public Task<Cart?> GetByIdAsync(long cartId);

        public Task<Cart?> GetByUserAsync(long userId);

        public Task<Cart?> UpdateAsync(Cart cart);
    }
}
