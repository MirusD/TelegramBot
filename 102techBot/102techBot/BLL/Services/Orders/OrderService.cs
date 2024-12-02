using _102techBot.BLL.Services.Carts;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;

namespace _102techBot.BLL.Services.Orders
{
    internal class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly CartService _cartService;

        public OrderService(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, CartService cartService)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _cartService = cartService;
        }

        public async Task<List<Order>> GetOrdersAsync(long userId)
        {
            return await _orderRepository.GetByUserAsync(userId);
        }

        public async Task<List<OrderItem>> GetOrderItemsAsync(long orderId)
        {
            return await _orderItemRepository.GetByOrder(orderId);
        }

        public async Task<Order?> ChangeStatusAsync(long orderId, OrderStatus status)
        {
            var repair = await _orderRepository.GetByIdAsync(orderId);
            repair!.Status = status;

            var updatedRepair = await _orderRepository.UpdateAsync(repair);
            if (updatedRepair != null)
            {
                return updatedRepair;
            }
            else
            {
                return null;
            }
        }

        public async Task<Order> AddOrderAsync(long userId)
        {
            var cartItems = await _cartService.GetCartItems(userId);
            var newOrder = new Order
            {
                Status = OrderStatus.Created,
                TotalAmount = 1,
                UserId = userId
            };

            var order = await _orderRepository.AddAsync(newOrder);

            foreach (var cartItem in cartItems)
            {
                var newOrderItem = new OrderItem
                {
                    Order = order,
                    OrderId = order.Id,
                    Price = cartItem.Product.Price,
                    Product = cartItem.Product,
                    ProductId = cartItem.ProductId,
                    Quantity = 1
                };
                await _orderItemRepository.AddAsync(newOrderItem);
            }

            return order;
        }

        public async Task<Order?> RemoveAsync(long id)
        {
            return await _orderRepository.DeleteAsync(id);
        }
    }
}
