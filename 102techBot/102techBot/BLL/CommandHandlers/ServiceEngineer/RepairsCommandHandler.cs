
using _102techBot.BLL.Services.Repairs;
using _102techBot.BLL.Services.Users;
using _102techBot.Common;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;
using _102techBot.Extensions;
using _102techBot.Utils;

namespace _102techBot.BLL.CommandHandlers.ServiceEngineer
{
    /// <summary>
    /// Обработка комманд связанных с обработкой заявок
    /// </summary>
    internal class RepairsCommandHandler : BaseCommandHandler
    {
        private readonly RepairService _repairService;
        public RepairsCommandHandler(
            IUI ui,
            ITemporaryStorageRepository temporaryStorageRepository,
            UserService userService,
            RepairService repairService) : base(ui, temporaryStorageRepository, userService)
        {
            _repairService = repairService;
        }

        public override async Task CommandHandler(User user, MessageDTO message, string command, string[] args)
        {
            switch (command)
            {
                case "/repairs":
                    var repairs = await _repairService.GetAllAsync();
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
                case "/repair_page":
                    var page = args[0];
                    if (int.TryParse(page, out int pageNumber))
                        await SendPaginationMessage(user, message, pageNumber);
                    else
                        await _UI.SendMessageAsync(
                            user.Id,
                            "Произошла ошибка, попробуйте повторить комманду /repairs"
                        );
                    break;
                case "/repair_change_status":
                    try
                    {
                        var repairId = args[0];
                        var setRepairStatus = args[1];

                        if (long.TryParse(repairId, out long id) && int.TryParse(setRepairStatus, out int status))
                        {
                            var changedStatusRepair = await _repairService.ChangeStatusAsync(id, (RepairStatus)status);

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
                case "/repair_remove":
                    try
                    {
                        var repairId = args[0];

                        if (long.TryParse(repairId, out long id))
                        {
                            var removedRepair = await _repairService.RemoveAsync(id);
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
            var tempRepairs = _storageRepository.Get<List<Repair>>(user.Id.ToString());

            if (tempRepairs != null)
            {
                await _UI.SendPaginatedMessage(
                    user.Id,
                    message.Id,
                    page,
                    tempRepairs,
                    repair => $"Заявка:\n{EntityFormatter.FormaRepairInfo(repair)}",
                    page =>
                    {
                        var repair = tempRepairs[page - 1];
                        var (buttonText, command) = GetButtonDetailsForRepairStatus(repair.Id, repair.Status);

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
                                Data = $"/repair_page/{page - 1}",
                            });
                        }

                        if (page < (int)Math.Ceiling(tempRepairs.Count / (double)1))
                        {
                            navBtns.Add(new Button
                            {
                                Text = "Вперёд ➡️",
                                Data = $"/repair_page/{page + 1}",
                            });
                        }
                        return navBtns;
                    });
            }
        }

        private (string buttonText, string command) GetButtonDetailsForRepairStatus(long repairId, RepairStatus status)
        {
            return status switch
            {
                RepairStatus.Created => ("Подтвердить", $"/repair_change_status/{repairId}/{(int)RepairStatus.Confirmed}"),
                RepairStatus.Confirmed => ("Взять в работу", $"/repair_change_status/{repairId}/{(int)RepairStatus.AtWork}"),
                RepairStatus.AtWork => ("Завершить", $"/repair_change_status/{repairId}/{(int)RepairStatus.Completed}"),
                RepairStatus.Completed => ("Закрыть", $"/repair_remove/{repairId}"),
                _ => (string.Empty, string.Empty) // Если статус не обрабатывается
            };
        }
    }
}
