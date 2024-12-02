using _102techBot.BLL.CommandHandlers.CommonCommandHandlers;
using _102techBot.BLL.Services.Callbacks;
using _102techBot.BLL.Services.Users;
using _102techBot.Common;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;

namespace _102techBot.BLL.CommandHandlers.Client
{
    /// <summary>
    /// Обработка комманд связанных с созданием заявки на обрытный звонок
    /// </summary>
    internal class CallbackCommandHandler : BaseCommandHandler
    {
        private readonly StartCommandHandler _startCommandHandler;
        private readonly CallbackService _callbackService;
        public CallbackCommandHandler(
            IUI ui,
            ITemporaryStorageRepository temporaryStorageRepository,
            UserService userService,
            StartCommandHandler startCommandHandler,
            CallbackService callbackService) : base(ui, temporaryStorageRepository, userService)
        {
            _startCommandHandler = startCommandHandler;
            _callbackService = callbackService;
        }

        public override async Task CommandHandler(User user, MessageDTO message, string command, string[] args)
        {
            switch (command)
            {
                case "/callback_create":
                    await _UI.EditMessageAsync(
                        user.Id,
                        message.Id,
                        $"Вы хотите создать заявку на обратный звонок?", 
                        Buttons.GetConfirmBtns("callback_create")
                    );
                    break;
                case "/callback_create_yes":
                    await _userService.ChangeStateAsync(user, UserState.AwaitingCallbackPhone);
                    await _UI.EditMessageAsync(user.Id, message.Id, "Укажите номер телефона в формате: +71234567890");
                    break;
                case "/callback_create_no":
                    await CommandHandler(user, message, "/callback_cancel", args);
                    break;
                case "/callback_send":
                    var tempCallbackRequest = _storageRepository.Get<Callback>(user.Id.ToString());

                    try
                    {
                        var newCallback = await _callbackService.AddCallbackAsync(tempCallbackRequest!);
                    }
                    catch(Exception) 
                    {
                        _storageRepository.Remove(user.Id.ToString());
                        await _UI.SendMessageAsync(
                            user.Id,
                            "Произошла ошибка при создании заявки на обратный звонок, попробуйте начать все с начала /start"
                        );
                    }

                    _storageRepository.Remove(user.Id.ToString());
                    await _UI.SendPushMessage(
                        message.CallbackQueryId,
                        "Заявка на обратный звонок отправлена, в ближайшее время с вами свяжется наш специалист"
                    );
                    await _startCommandHandler.CommandHandler(user, message, "/main_menu", args);
                    break;
                case "/callback_cancel":
                    _storageRepository.Remove(user.Id.ToString());
                    await _UI.SendPushMessage(message.CallbackQueryId, "Заявка на обратный звонок отменена");
                    await _startCommandHandler.CommandHandler(user, message, "/main_menu", args);
                    break;

                default:
                    await _UI.SendMessageAsync(
                        user.Id,
                        $"Нет такой комманды \"{command}\". Вам доступна комманад /callback_create"
                    );
                    break;
            }
        }
    }
}
