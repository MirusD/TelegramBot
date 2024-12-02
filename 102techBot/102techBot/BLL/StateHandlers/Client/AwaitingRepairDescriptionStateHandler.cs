using _102techBot.BLL.Services.Users;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;
using AutoMapper;

namespace _102techBot.BLL.StateHandlers.Client
{
    /// <summary>
    /// Обработка состояния ожидания ввода описания неисправности
    /// </summary>
    internal class AwaitingRepairDescriptionStateHandler : BaseStateHandler
    {
        private readonly UserService _userService;
        public AwaitingRepairDescriptionStateHandler(
            IUI ui,
            ITemporaryStorageRepository temporaryStorageRepository,
            IMapper mapper,
            UserService userService) : base(ui, temporaryStorageRepository, mapper) 
        {
            _userService = userService;
        }

        public override async Task HandleStateAsync(User user, MessageDTO message)
        {
            if (!string.IsNullOrEmpty(message.Text))
            {
                var tempRepair = _storageRepository.Get<TemporaryRepair>(user.Id.ToString());
                if (tempRepair != null)
                {
                    tempRepair.Description = message.Text;
                }

                _storageRepository.Add(user.Id.ToString(), tempRepair);

                await _userService.ChangeStateAsync(user, UserState.AwaitingRepairPhone);
                await _UI.SendMessageAsync(user.Id, "Введите номер телефона в формате +71234567890:");
            }
            else
            {
                await _UI.SendPushMessage(message.CallbackQueryId, "Вы отправили пустую строку, пожалуйста опишите свою проблему");
            }
        }
    }
}
