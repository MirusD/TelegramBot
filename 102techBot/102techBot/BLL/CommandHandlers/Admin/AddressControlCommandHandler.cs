using _102techBot.BLL.CommandHandlers.CommonCommandHandlers;
using _102techBot.BLL.Services.Addresses;
using _102techBot.BLL.Services.Users;
using _102techBot.Common;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;

namespace _102techBot.BLL.CommandHandlers.Admin
{
    /// <summary>
    /// Обработка комманд связанных с работой над адресами
    /// </summary>
    internal class AddressControlCommandHandler : BaseCommandHandler
    {
        private readonly StartCommandHandler _startCommandHandler;
        private readonly AddressService _addressService;
        public AddressControlCommandHandler(
            IUI ui,
            ITemporaryStorageRepository temporaryStorageRepository,
            StartCommandHandler startCommandHandler,
            AddressService addressService,
            UserService userService) : base(ui, temporaryStorageRepository, userService) 
        {
            _startCommandHandler = startCommandHandler;
            _addressService = addressService;
        }

        public override async Task CommandHandler(User user, MessageDTO message, string command, string[] args)
        {
            switch (command)
            {
                case "/address_create":
                    await _UI.EditMessageAsync(
                        user.Id, 
                        message.Id, 
                        "Вы хотите добавить новый адрес?", 
                        Buttons.GetConfirmBtns("address_create")
                    );
                    break;
                case "/address_create_yes":
                    await _userService.ChangeStateAsync(user, UserState.AwaitingAddressCountry);
                    await _UI.EditMessageAsync(user.Id, message.Id, "В какой стране находиться офис?");
                    break;
                case "/address_create_no":
                    await _startCommandHandler.CommandHandler(user, message, "/main_menu", args);
                    break;
                case "/address_create_ok":
                    var newAddress = _storageRepository.Get<Address>(user.Id.ToString());
                    if (newAddress != null)
                    {
                        try
                        {
                            await _addressService.AddAddressAsync(newAddress);
                            await _UI.SendPushMessage(message.CallbackQueryId, "Адрес добавлено в базу данных");
                            await _startCommandHandler.CommandHandler(user, message, "/main_menu", args);

                        } catch (Exception ex)
                        {
                            await _UI.SendMessageAsync(
                                user.Id, 
                                "Произошла ошибка при добавлении нового адреса, " +
                                "попробуйте все повторить /address_create"
                            );
                        }
                    }
                    else
                    {
                        await _UI.SendMessageAsync(
                            user.Id,
                            "Возника ошибка при добавлении новго адреса. " +
                            "Попробуйте все повторить /address_create"
                        );
                    }
                    break;
                case "/address_create_cancel":
                    await _startCommandHandler.CommandHandler(user, message, "/main_menu", args);
                    break;
            }
        }
    }
}
