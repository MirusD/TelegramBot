using _102techBot.BLL.Services.Users;
using _102techBot.Common;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;
using _102techBot.Utils;
using AutoMapper;

namespace _102techBot.BLL.StateHandlers.WorkWithAddress.Admin
{
    /// <summary>
    /// Обработка состояния ожидания ввода режима работы офиса при добавлении нового адреса в систему
    /// </summary>
    internal class AwaitingAddressWorkingHoursStateHandler : BaseStateHandler
    {
        private readonly UserService _userService;
        public AwaitingAddressWorkingHoursStateHandler(
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
                newAddress!.WorkingHours = message.Text;

                var createdNewAddress = _mapper.Map<Address>(newAddress);

                _storageRepository.Add(user.Id.ToString(), createdNewAddress);

                await _UI.SendMessageAsync(
                user.Id,
                "== Заявка сформирована ==\n" +
                EntityFormatter.FormatAddressInfo(createdNewAddress),
                new List<Button>()
                {
                    new Button {
                        Text = "Добавить",
                        Data = "/address_create_ok",
                        Ln = true
                    },
                    new Button {
                        Text = "Отменить",
                        Data = "/address_create_cancel",
                        Ln = true
                    },
                });
                await _userService.ChangeStateAsync(user, UserState.None);
            }
            else
            {
                await _UI.SendPushMessage(message.CallbackQueryId, "Вы отправили пустую строку, пожалуйста введите режим работы");
            }
        }
    }
}
