using _102techBot.BLL.Services.Orders;
using _102techBot.BLL.Services.Users;
using _102techBot.Common;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;
using _102techBot.Extensions;
using _102techBot.Utils;

namespace _102techBot.BLL.CommandHandlers.SalesManager
{
    /// <summary>
    /// Обработка комманд связанных с обработкой заявок
    /// </summary>
    internal class OrdersCommandHandler : BaseCommandHandler
    {
        private readonly OrderService _orderService;
        public OrdersCommandHandler(
            IUI ui,
            ITemporaryStorageRepository temporaryStorageRepository,
            UserService userService,
            OrderService orderService) : base(ui, temporaryStorageRepository, userService)
        {
            _orderService = orderService;
        }

        public override async Task CommandHandler(User user, MessageDTO message, string command, string[] args)
        {
            switch (command)
            {
                case "/orders":
                    var repairs = await _orderService.GetOrdersAsync(user.Id);
                    if (repairs.Count == 0)
                    {
                        await _UI.SendPushMessage(
                            message.CallbackQueryId,
                            "Нет заявок на ремонт");
                    }
                    else
                    {
                        _storageRepository.Add(user.Id.ToString(), repairs);
                        await SendPaginationMessage(user, message, 1);
                    }
                    break;
                case "/order_page":
                    var page = args[0];
                    if (int.TryParse(page, out int pageNumber))
                        await SendPaginationMessage(user, message, pageNumber);
                    else
                        await _UI.SendMessageAsync(
                            user.Id,
                            "Произошла ошибка, попробуйте повторить комманду /orders"
                        );
                    break;
                case "/order_change_status":
                    try
                    {
                        var repairId = args[0];
                        var setRepairStatus = args[1];

                        if (long.TryParse(repairId, out long id) && int.TryParse(setRepairStatus, out int status))
                        {
                            var changedStatusRepair = await _orderService.ChangeStatusAsync(id, (OrderStatus)status);

                            if (changedStatusRepair != null)
                            {
                                await _UI.SendPushMessage(
                                    callbackQueryId: message.CallbackQueryId,
                                    text: $"Статус заявки изменён на: " +
                                    $"{changedStatusRepair.Status.GetDescription()}"
                                );
                            }
                        }
                        else
                        {
                            await _UI.SendPushMessage(
                                callbackQueryId: message.CallbackQueryId,
                                text: "Возникла ошибка при изменении статуса заявки"
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        await _UI.SendPushMessage(
                            callbackQueryId: message.CallbackQueryId,
                            text: "Возникла ошибка при изменении статуса заявки"
                        );
                    }
                    break;
                case "/order_remove":
                    try
                    {
                        var repairId = args[0];

                        if (long.TryParse(repairId, out long id))
                        {
                            var removedRepair = await _orderService.RemoveAsync(id);
                            if (removedRepair != null)
                            {
                                await _UI.SendPushMessage(
                                    callbackQueryId: message.CallbackQueryId,
                                    text: "Заявка закрыта.");
                            }
                        }
                        else
                        {
                            await _UI.SendPushMessage(
                                callbackQueryId: message.CallbackQueryId,
                                text: "Возникла ошибка при закрытии заявки");
                        }
                    }
                    catch (Exception ex)
                    {
                        await _UI.SendPushMessage(
                            callbackQueryId: message.CallbackQueryId,
                            text: "Возникла ошибка при закрытии заявки");
                    }
                    break;
            }
        }

        private async Task SendPaginationMessage(User user, MessageDTO message, int page)
        {
            var tempOrders = _storageRepository.Get<List<Order>>(user.Id.ToString());

            if (tempOrders != null)
            {
                await _UI.SendPaginatedMessage(
                    user.Id,
                    message.Id,
                    page,
                    tempOrders,
                    order => $"Заявка:\n{EntityFormatter.FormaOrderInfo(order)}",
                    page =>
                    {
                        var order = tempOrders[page - 1];
                        var (buttonText, command) = GetButtonDetailsForOrderStatus(order.Id, order.Status);

                        var navBtns = new List<Button>()
                        {
                            new Button {
                                Text = buttonText,
                                Data = command,
                                Ln = true
                            },
                            new Button {
                                Text = "< Назад в меню",
                                Data = "/main_menu",
                                Ln = true
                            }
                        };

                        if (page > 1)
                        {
                            navBtns.Add(new Button
                            {
                                Text = "⬅️ Назад",
                                Data = $"/order_page/{page - 1}",
                            });
                        }

                        if (page < (int)Math.Ceiling(tempOrders.Count / (double)1))
                        {
                            navBtns.Add(new Button
                            {
                                Text = "Вперёд ➡️",
                                Data = $"/order_page/{page + 1}",
                            });
                        }
                        return navBtns;
                    });
            }
        }

        private (string buttonText, string command) GetButtonDetailsForOrderStatus(long orderId, OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Created => ("Подтвердить", $"/order_change_status/{orderId}/{(int)RepairStatus.Confirmed}"),
                OrderStatus.Confirmed => ("Взять в работу", $"/order_change_status/{orderId}/{(int)RepairStatus.AtWork}"),
                OrderStatus.AtWork => ("Завершить", $"/order_change_status/{orderId}/{(int)RepairStatus.Completed}"),
                OrderStatus.Completed => ("Закрыть", $"/order_remove/{orderId}"),
                _ => (string.Empty, string.Empty)
            };
        }
    }
}
