using _102techBot.BLL.CommandHandlers.NewClient;
using _102techBot.BLL.Interfaces;
using _102techBot.BLL.Services.Users;
using _102techBot.BLL.StateHandlers.Admin;
using _102techBot.BLL.StateHandlers.Client;
using _102techBot.BLL.StateHandlers.NewClient;
using _102techBot.BLL.StateHandlers.WorkWithAddress.Admin;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace _102techBot.BLL.Routers
{
    internal class StateRouter
    {
        private readonly CommandRouter _commandRouter;
        private readonly IServiceProvider _serviceProvider;
        //private readonly Dictionary<UserState, IStateHandler> _stateHandlers;
        private readonly Dictionary<UserRole, Dictionary<UserState, IStateHandler>> _stateHandlers;
        private readonly UserService _userService;
        private readonly IUI _UI;

        public StateRouter(IServiceProvider serviceProvider, CommandRouter commandRouter, IUI ui, UserService userService)
        {
            _serviceProvider = serviceProvider;
            _commandRouter = commandRouter;
            _userService = userService;
            _UI = ui;
            _UI.HandlersOnMessage += RouteAsync;
            _UI.HandlersOnUpdate += RouteAsync;

            // Регистрация обработчиков состояний
            
            _stateHandlers = new Dictionary<UserRole, Dictionary<UserState, IStateHandler>>
            {
                {
                    UserRole.NewClient,
                    new Dictionary<UserState, IStateHandler>
                    {
                        { UserState.AwaitingEnterName, serviceProvider.GetRequiredService<AwaitingNameStateHandler>() },
                    }
                },
                {
                    UserRole.Administrator,
                    new Dictionary<UserState, IStateHandler>
                    {
                        { UserState.AwaitingCategoryName, serviceProvider.GetRequiredService<AwaitingCategoryNameStateHandler>() },

                        { UserState.AwaitingProductName, serviceProvider.GetRequiredService<AwaitingProductNameStateHandler>() },
                        { UserState.AwaitingProductDescription, serviceProvider.GetRequiredService<AwaitingProductDescriptionStateHandler>() },
                        { UserState.AwaitingProductPrice, serviceProvider.GetRequiredService<AwaitingProductPriceStateHandler>() },
                        { UserState.AwaitingProductStock, serviceProvider.GetRequiredService<AwaitingProductStockStateHandler>() },
                        { UserState.AwaitingProductPhoto, serviceProvider.GetRequiredService<AwaitingProductPhotoStateHandler>() },

                        { UserState.AwaitingAddressCountry, serviceProvider.GetRequiredService<AwaitingAddressCountryStateHandler>() },
                        { UserState.AwaitingAddressCity, serviceProvider.GetRequiredService<AwaitingAddressCityStateHandler>() },
                        { UserState.AwaitingAddressStreet, serviceProvider.GetRequiredService<AwaitingAddressStreetStateHandler>() },
                        { UserState.AwaitingAddressLatitude, serviceProvider.GetRequiredService<AwaitingAddressLatitudeStateHandler>() },
                        { UserState.AwaitingAddressLongitude, serviceProvider.GetRequiredService<AwaitingAddressLongitudeStateHandler>() },
                        { UserState.AwaitingAddressPhone, serviceProvider.GetRequiredService<AwaitingAddressPhoneStateHandler>() },
                        { UserState.AwaitingAddressPostalCode, serviceProvider.GetRequiredService<AwaitingAddressPostalCodeStateHandler>() },
                        { UserState.AwaitingAddressWorkingHours, serviceProvider.GetRequiredService<AwaitingAddressWorkingHoursStateHandler>() },

                    }
                },
                {
                    UserRole.Client,
                    new Dictionary<UserState, IStateHandler>
                    {
                        { UserState.AwaitingRepairDescription, serviceProvider.GetRequiredService<AwaitingRepairDescriptionStateHandler>() },
                        { UserState.AwaitingRepairPhone, serviceProvider.GetRequiredService<AwaitingRepairPhoneStateHandler>() },

                        { UserState.AwaitingCallbackPhone, serviceProvider.GetRequiredService<AwaitingCallbackPhoneStateHandler>() },
                    }
                }
            };
        }

        public async Task RouteAsync(User user, MessageDTO message)
        {
            user = await _userService.CheckAndGetActualUser(user);
            // Если сообщение является командой
            if (message.Text != null && message.Text.StartsWith("/"))
            {
                await _commandRouter.RouteCommandAsync(user, message, message.Text);
            }
            else
            {
                // Если это обычный текст, проверяем состояние
                if (_stateHandlers.TryGetValue(user.Role, out var handlers) && handlers.TryGetValue(user.State, out var handler))
                {
                    await handler.HandleStateAsync(user, message);
                }
                else
                {
                    // Обработка если состояние неизвестно
                    await _UI.SendMessageAsync(user.Id, "Я не понимаю, что вы хотите.");
                }
            }
        }
    }
}
