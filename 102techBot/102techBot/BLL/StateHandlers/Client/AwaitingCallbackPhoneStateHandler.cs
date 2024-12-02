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
    /// Обработка состояни ожидания ввода номера телефона
    /// </summary>
    internal class AwaitingCallbackPhoneStateHandler : BaseStateHandler
    {
        private readonly UserService _userService;
        public AwaitingCallbackPhoneStateHandler(
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

                var tempCallbackRequest = new Callback
                {
                    UserId = user.Id,
                    Phone = message.Text.Trim(),
                    Status = CallbackStatus.Created
                };

                _storageRepository.Add(user.Id.ToString(), tempCallbackRequest);

                if (tempCallbackRequest != null)
                {
                    await _userService.ChangeStateAsync(user, UserState.None);

                    await _UI.SendMessageAsync(
                    user.Id,
                    "== Заявка сформирована ==\n" +
                    EntityFormatter.FormatCallbackInfo(message.Text),
                    new List<Button>()
                    {
                    new Button {
                        Text = "Отправить",
                        Data = "/callback_send",
                        Ln = true
                    },
                    new Button {
                        Text = "Отменить",
                        Data = "/callback_cancel",
                        Ln = true
                    },
                    });
                }
            }
            else
            {
                await _UI.SendPushMessage(message.CallbackQueryId, "Вы отправили пустую строку, пожалуйста введите номер телефона");
            }
        }
    }
}
