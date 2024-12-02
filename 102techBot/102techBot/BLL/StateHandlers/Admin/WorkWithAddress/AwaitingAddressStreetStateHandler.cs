using _102techBot.BLL.Services.Users;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;
using AutoMapper;

namespace _102techBot.BLL.StateHandlers.WorkWithAddress.Admin
{
    /// <summary>
    /// Обработка состояния ожидания ввода названия улицы при добавлении нового адреса в систему
    /// </summary>
    internal class AwaitingAddressStreetStateHandler : BaseStateHandler
    {
        private readonly UserService _userService;
        public AwaitingAddressStreetStateHandler(
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
                var newAddress = _storageRepository.Get<AddressDTO>(user.Id.ToString());
                newAddress!.Street = message.Text;
                _storageRepository.Add(user.Id.ToString(), newAddress);

                await _UI.SendMessageAsync(user.Id, $"Почтовый индекс:");
                await _userService.ChangeStateAsync(user, UserState.AwaitingAddressPostalCode);
            }
            else
            {
                await _UI.SendPushMessage(message.CallbackQueryId, "Вы отправили пустую строку, пожалуйста введите название улицы");
            }
        }
    }
}
