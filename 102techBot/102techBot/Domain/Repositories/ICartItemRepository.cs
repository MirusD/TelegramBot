using _102techBot.Domain.Entities;

namespace _102techBot.Domain.Repositories
{
    internal interface ICartItemRepository
    {
        public Task<CartItem> AddAsync(CartItem newCartItem);
        public Task<CartItem?> DeleteAsync(long cartItemId);
        public Task<int> DeleteAllByCart(long cartId);
        public Task<List<CartItem>> GetAllAsync();
        public Task<CartItem?> GetByIdAsync(long cartItemId);
        public Task<List<CartItem>> GetByCartAsync(long cartId);
        public Task<CartItem?> UpdateAsync(CartItem cartItem);
    }
}
