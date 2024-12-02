using _102techBot.BLL.Services.Products;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;

namespace _102techBot.BLL.Services.Carts
{
    internal class CartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly ProductService _productService;
        public CartService(ICartRepository cartRepository, ProductService productService, ICartItemRepository cartItemRepository)
        {
            _cartRepository = cartRepository;
            _productService = productService;
            _cartItemRepository = cartItemRepository;
        }

        public async Task<Cart> CreateCart(Cart cart)
        {
            return await _cartRepository.AddAsync(cart);
        }
        public async Task<CartItem?> AddProductAsync(long userId, long productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            var cart = await _cartRepository.GetByUserAsync(userId);

;           if (product != null && cart != null)
            {
                var newCartItem = new CartItem { CartId = cart.Id, ProductId = product.Id, Quantity = 1 };
                return await _cartItemRepository.AddAsync(newCartItem);
            }
            return null;
        }
        public async Task<List<CartItem>?> GetCartItems(long userId)
        {
            try
            {
                var cart = await _cartRepository.GetByUserAsync(userId);
                return await _cartItemRepository.GetByCartAsync(cart!.Id);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении товара из корзины: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> ClearAsync(long userId)
        {
            var cart = await _cartRepository.GetByUserAsync(userId);
            if (cart == null) return false;
            await _cartItemRepository.DeleteAllByCart(cart.Id);
            return true;
        }

        public async Task<CartItem?> RemoveCartItem(long id)
        {
            return await _cartItemRepository.DeleteAsync(id);
        }
    }
}
