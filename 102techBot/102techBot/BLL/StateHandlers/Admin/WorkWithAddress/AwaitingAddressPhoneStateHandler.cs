using _102techBot.BLL.Services.Users;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;
using _102techBot.Utils;
using AutoMapper;

namespace _102techBot.BLL.StateHandlers.WorkWithAddress.Admin
{
    /// <summary>
    /// Обработка состояния ожидания ввода номера телефона при добавлении нового адреса в систему
    /// </summary>
    internal class AwaitingAddressPhoneStateHandler : BaseStateHandler
    {
        private readonly UserService _userService;
        public AwaitingAddressPhoneStateHandler(
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
                if (!Validator.IsValidRussianPhoneNumber(message.Text.Trim()))
                {
                    await _UI.SendMessageAsync(
                        user.Id,
                        "Введенный номер телефона не корректен\n" +
                        "Пожалуйста, введите номер телефона в формате: +71234567890");

                    return;
                }

                var newAddress = _storageRepository.Get<AddressDTO>(user.Id.ToString());
                newAddress!.Phone = message.Text;
                _storageRepository.Add(user.Id.ToString(), newAddress);

                await _UI.SendMessageAsync(user.Id, $"Режим работы:");
                await _userService.ChangeStateAsync(user, UserState.AwaitingAddressWorkingHours);
            }
            else
            {
                await _UI.SendPushMessage(message.CallbackQueryId, "Вы отправили пустую строку, пожалуйста введите номер телефона");
            }
        }
    }
}
