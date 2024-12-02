using _102techBot.BLL.CommandHandlers.CommonCommandHandlers;
using _102techBot.BLL.Services.Addresses;
using _102techBot.BLL.Services.Callbacks;
using _102techBot.BLL.Services.Users;
using _102techBot.Common;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;
using _102techBot.Utils;

namespace _102techBot.BLL.CommandHandlers.Client
{
    /// <summary>
    /// Обработка комманд связанных c просмотром контактных данных магазинов и сервисов 
    /// </summary>
    internal class ShopsAndServicesCommandHandler : BaseCommandHandler
    {
        private readonly StartCommandHandler _startCommandHandler;
        private readonly AddressService _addressService;
        public ShopsAndServicesCommandHandler(
            IUI ui,
            ITemporaryStorageRepository temporaryStorageRepository,
            UserService userService,
            StartCommandHandler startCommandHandler,
            AddressService addressService) : base(ui, temporaryStorageRepository, userService)
        {
            _startCommandHandler = startCommandHandler;
            _addressService = addressService;
        }

        public override async Task CommandHandler(User user, MessageDTO message, string command, string[] args)
        {
            switch (command)
            {
                case "/shops_services":
                    var addresses = await _addressService.GetAddressesAsync();
                    if (addresses.Count != 0)
                    {
                        _storageRepository.Add(user.Id.ToString(), addresses);
                        await SendPaginationMessage(user, message, 1);
                    }
                    else
                    {
                        await _UI.SendPushMessage(
                            message.CallbackQueryId, 
                            "На данный момент адресов нет"
                        );
                    }
                    break;
                case "/shops_services_page":
                    var page = args[0];
                    if (int.TryParse(page, out int pageNumber))
                    {
                        await SendPaginationMessage(user, message, pageNumber);
                    }
                    else
                    {
                        await _UI.SendMessageAsync(
                            user.Id, 
                            "Произошло ошибка повторите комманду /shops_services"
                        );
                    }
                    break;
            }
        }

        private async Task SendPaginationMessage(User user, MessageDTO message, int page)
        {
            var addresses = _storageRepository.Get<List<Address>>(user.Id.ToString());

            if (addresses == null || addresses.Count == 0) 
            {
                await _UI.SendMessageAsync(
                    user.Id, 
                    "Произошло ошибка повторите комманду /shops_services"
                );
                return;
            }

            await _UI.SendPaginatedMessage(
                user.Id,
                message.Id,
                page,
                addresses,
                address => $"Адрес:\n{EntityFormatter.FormatAddressInfo(address)}",
                page => {
                    var navBtns = new List<Button>
                        {
                            new Button {
                                Text = "Яндекс карта",
                                Link = $"https://yandex.ru/maps/?ll={addresses[page - 1].Longitude}," +
                                $"{addresses[page - 1].Latitude}&z=12",
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
                            Data = $"/shops_services_page/{page - 1}",
                        });
                    }

                    if (page < (int)Math.Ceiling(addresses.Count / (double)1))
                    {
                        navBtns.Add(new Button
                        {
                            Text = "Вперёд ➡️",
                            Data = $"/shops_services_page/{page + 1}",
                        });
                    }

                    return navBtns;
                },
                itemsPerPage: 1);
        }
    }
}
