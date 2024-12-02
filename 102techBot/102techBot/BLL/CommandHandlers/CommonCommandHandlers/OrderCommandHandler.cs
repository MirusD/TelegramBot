using _102techBot.BLL.Interfaces;
using _102techBot.BLL.Services.Carts;
using _102techBot.BLL.Services.Orders;
using _102techBot.BLL.Services.Users;
using _102techBot.Common;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;
using _102techBot.Utils;

namespace _102techBot.BLL.CommandHandlers.CommonCommandHandlers
{
    /// <summary>
    /// Обработка комманд связанных с заказом товаров
    /// </summary>
    internal class OrderCommandHandler : BaseCommandHandler
    {
        private readonly CartService _cartService;
        private readonly OrderService _orderService;
        private readonly INotificationService _notificationService;
        public OrderCommandHandler(
            IUI ui,
            ITemporaryStorageRepository temporaryStorageRepository,
            UserService userService,
            CartService cartService,
            OrderService orderService,
            INotificationService notificationService) : base(ui, temporaryStorageRepository, userService)
        {
            _cartService = cartService;
            _orderService = orderService;
            _notificationService = notificationService;
        }

        public override async Task CommandHandler(User user, MessageDTO message, string command, string[] args)
        {
            switch (command)
            {
                case "/orders":
                    var orderItems = await _orderService.GetOrderItemsAsync(user.Id);
                    if (orderItems.Count != 0)
                    {
                        _storageRepository.Add(user.Id.ToString(), orderItems);
                        await SendPaginationMessage(user, message, 1);
                    }
                    else
                    {
                        await _UI.SendPushMessage(message.CallbackQueryId, "Нет активных заказов.");
                    }
                    break;
                case "/order":
                    var selectOrderId = args[0];
                    if (int.TryParse(selectOrderId, out int orderId))
                    {

                    }
                    break;
                case "/order_add":
                    var cartItems = await _cartService.GetCartItems(user.Id);
                    if (cartItems == null)
                    {
                        await _UI.SendPushMessage(message.CallbackQueryId, "Произошла ошибка мы разбираемся");
                        return;
                    }
                    if (cartItems.Count != 0)
                    {
                        var newOrder = await _orderService.AddOrderAsync(user.Id);
                        if (newOrder != null)
                        {
                            if (await _cartService.ClearAsync(user.Id))
                            {
                                await _UI.SendPushMessage(message.CallbackQueryId, "Заявка на покупку отправлена");
                                await _notificationService.NotifyUsersByRolesAsync(
                                    new List<UserRole> { UserRole.SalesManager, UserRole.Administrator},
                                    "Добавлена новая заявка на покупку"
                                );
                            }
                        }
                    }
                    else
                    {
                        await _UI.SendPushMessage(message.CallbackQueryId, "В корзине нет товаров для формирования заказа");
                    }
                    break;
                case "/order_remove":
                    break;
                case "/order_page":
                    var page = args[0];
                    if (int.TryParse(page, out int pageNumber))
                    {
                        await SendPaginationMessage(user, message, pageNumber);
                    }
                    else
                    {
                        await _UI.SendPushMessage(message.CallbackQueryId, "Не удалось получить номер страницы, попробуйте еще раз");
                    }
                    break;
            }
        }

        private async Task SendPaginationMessage(User user, MessageDTO message, int page)
        {
            var tempOrderItems = _storageRepository.Get<List<OrderItem>>(user.Id.ToString());

            if (tempOrderItems != null)
            {
                await _UI.RemoveMessageAsync(user.Id, message.Id);
                await _UI.SendPhotoPaginatedMessage(
                    user.Id,
                    message.Id,
                    page,
                    tempOrderItems,
                    orderItem => $"{EntityFormatter.FormatProductInfo(orderItem.Product)}",
                    orderItem => Path.Combine(AppContext.BaseDirectory, "Img", orderItem.Product?.ImageUrl == null ? "" : orderItem.Product.ImageUrl),
                    orderItem => new List<Button>
                        {
                            new Button 
                            {
                                Text = "Удалить",
                                Data = $"/order_remove/{orderItem.Id}",
                                Ln = true
                            }
                        },
                    page => {
                        var btns = new List<Button>
                        {
                            new Button
                            {
                                Text = "< Назад",
                                Data = $"/main_menu",
                                Ln = true
                            }
                        };

                        if (page < (int)Math.Ceiling(tempOrderItems.Count / (double)3))
                        {
                            btns.Add(new Button
                            {
                                Text = "⬇️ Смотреть еще ⬇️",
                                Data = $"/order_page/{page + 1}",
                                Ln = true
                            }); ;
                        }

                        return btns;
                    },     
                    itemsPerPage: 3);
            }
        }
    }
}
