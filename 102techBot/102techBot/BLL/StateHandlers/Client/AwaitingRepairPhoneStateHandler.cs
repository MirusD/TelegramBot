using _102techBot.BLL.Services.Repairs;
using _102techBot.BLL.Services.Users;
using _102techBot.Common;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;
using _102techBot.Utils;
using AutoMapper;

namespace _102techBot.BLL.StateHandlers.Client
{
    /// <summary>
    /// Обработка состояния ожидания ввода номера телефона при создании заявки на ремонт
    /// </summary>
    internal class AwaitingRepairPhoneStateHandler : BaseStateHandler
    {
        private readonly UserService _userService;
        private readonly RepairService _repairService;
        public AwaitingRepairPhoneStateHandler(
            IUI ui,
            ITemporaryStorageRepository temporaryStorageRepository,
            IMapper mapper,
            UserService userService,
            RepairService repairService) : base(ui, temporaryStorageRepository, mapper) 
        {
            _userService = userService;
            _repairService = repairService;
        }

        public override async Task HandleStateAsync(User user, MessageDTO message)
        {
            if (!string.IsNullOrEmpty(message.Text))
            {
                if (!Validator.IsValidRussianPhoneNumber(message.Text.Trim()))
                {
                    await _UI.SendMessageAsync(
                        user.Id,
                        "Введенный номер телефона не корректен\n" +
                        "Пожалуйста, введите номер телефона в формате: +71234567890");

                    return;
                }

                var tempRepair = _storageRepository.Get<TemporaryRepair>(user.Id.ToString());
                if (tempRepair != null)
                {
                    tempRepair.Phone = message.Text;
                    tempRepair.Status = RepairStatus.Created;
                }

                var createdRepairRequest = _mapper.Map<Repair>(tempRepair);

                _storageRepository.Add(user.Id.ToString(), tempRepair);

                await _userService.ChangeStateAsync(user, UserState.None);

                await _UI.SendMessageAsync(
                user.Id,
                "== Заявка сформирована ==\n" +
                EntityFormatter.FormaRepairInfo(createdRepairRequest),
                new List<Button>()
                {
                    new Button {
                        Text = "Отправить",
                        Data = "/repair_send",
                        Ln = true
                    },
                    new Button {
                        Text = "Отменить",
                        Data = "/repair_cancel",
                        Ln = true
                    },
                });
            }
            else
            {
                await _UI.SendPushMessage(message.CallbackQueryId, "Вы отправили пустую строку, пожалуйста введите номер телефона");
            }
        }
    }
}
