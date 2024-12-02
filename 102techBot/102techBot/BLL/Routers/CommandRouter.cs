using _102techBot.BLL.CommandHandlers.NewClient;
using _102techBot.BLL.CommandHandlers.CommonCommandHandlers;
using _102techBot.BLL.Interfaces;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.DTOs;
using _102techBot.Extensions;
using _102techBot.Utils;
using Microsoft.Extensions.DependencyInjection;
using _102techBot.BLL.CommandHandlers.Admin;
using _102techBot.BLL.CommandHandlers.Client;
using _102techBot.BLL.CommandHandlers.ServiceEngineer;
using _102techBot.BLL.CommandHandlers.SalesManager;

namespace _102techBot.BLL.Routers
{
    internal class CommandRouter
    {
        private readonly Dictionary<UserRole, Dictionary<string, ICommandHandler>> _roleHandlers;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUI _UI;
        public CommandRouter(IServiceProvider serviceProvider, IUI ui)
        {
            _serviceProvider = serviceProvider;
            _UI = ui;

            _roleHandlers = new Dictionary<UserRole, Dictionary<string, ICommandHandler>>
            {
                {
                    UserRole.NewClient,
                    new Dictionary<string, ICommandHandler>
                    {
                        { "/start", serviceProvider.GetRequiredService<CommandHandlers.NewClient.StartCommandHandler>() },

                        { "/confirm_name_yes", serviceProvider.GetRequiredService<ConfirmNameCommandHandler>() },
                        { "/confirm_name_no", serviceProvider.GetRequiredService<ConfirmNameCommandHandler>() },
                    }
                },
                {
                    UserRole.Client,
                    new Dictionary<string, ICommandHandler>
                    {
                        { "/start", serviceProvider.GetRequiredService<CommandHandlers.CommonCommandHandlers.StartCommandHandler>() },
                        { "/main_menu", serviceProvider.GetRequiredService<CommandHandlers.CommonCommandHandlers.StartCommandHandler>() },

                        { "/catalog", serviceProvider.GetRequiredService<CatalogCommandHandler>() },
                        { "/category", serviceProvider.GetRequiredService<CatalogCommandHandler>() },
                        { "/page", serviceProvider.GetRequiredService<CatalogCommandHandler>() },

                        { "/cart", serviceProvider.GetRequiredService<CartCommandHandler>() },
                        { "/cart_add", serviceProvider.GetRequiredService<CartCommandHandler>() },
                        { "/cart_remove", serviceProvider.GetRequiredService<CartCommandHandler>() },
                        { "/cart_page", serviceProvider.GetRequiredService<CartCommandHandler>() },

                        { "/order_add", serviceProvider.GetRequiredService<OrderCommandHandler>() },

                        { "/repair_create", serviceProvider.GetRequiredService<RepairCommandHandler>() },
                        { "/repair_create_select_category", serviceProvider.GetRequiredService<RepairCommandHandler>() },
                        { "/repair_create_select_model", serviceProvider.GetRequiredService<RepairCommandHandler>() },
                        { "/repair_send", serviceProvider.GetRequiredService<RepairCommandHandler>() },
                        { "/repair_cancel", serviceProvider.GetRequiredService<RepairCommandHandler>() },

                        { "/callback_create", serviceProvider.GetRequiredService<CallbackCommandHandler>()},
                        { "/callback_create_yes", serviceProvider.GetRequiredService<CallbackCommandHandler>()},
                        { "/callback_create_no", serviceProvider.GetRequiredService<CallbackCommandHandler>()},
                        { "/callback_send", serviceProvider.GetRequiredService<CallbackCommandHandler>()},
                        { "/callback_cancel", serviceProvider.GetRequiredService<CallbackCommandHandler>()},


                        { "/shops_services", serviceProvider.GetRequiredService<ShopsAndServicesCommandHandler>()},
                        { "/shops_services_page", serviceProvider.GetRequiredService<ShopsAndServicesCommandHandler>()},

                        { "/search_job_add_candidate", serviceProvider.GetRequiredService<SearchJobCommandHandler>() }
                    }
                },
                {
                    UserRole.Administrator,
                    new Dictionary<string, ICommandHandler>
                    {
                        { "/start", serviceProvider.GetRequiredService<CommandHandlers.CommonCommandHandlers.StartCommandHandler>() },
                        { "/main_menu", serviceProvider.GetRequiredService<CommandHandlers.CommonCommandHandlers.StartCommandHandler>() },

                        { "/catalog", serviceProvider.GetRequiredService<CatalogCommandHandler>() },
                        { "/category", serviceProvider.GetRequiredService<CatalogCommandHandler>() },
                        { "/page", serviceProvider.GetRequiredService<CatalogCommandHandler>() },

                        { "/users_control", serviceProvider.GetRequiredService<UserControlCommandHandler>() },
                        { "/users", serviceProvider.GetRequiredService<UserControlCommandHandler>() },
                        { "/role_change", serviceProvider.GetRequiredService<UserControlCommandHandler>() },
                        { "/select_role", serviceProvider.GetRequiredService<UserControlCommandHandler>() },

                        { "/products_control", serviceProvider.GetRequiredService<ProductControlCommandHandler>() },
                        { "/add_category", serviceProvider.GetRequiredService<ProductControlCommandHandler>() },
                        { "/add_product", serviceProvider.GetRequiredService<ProductControlCommandHandler>() },
                        { "/add_product_select_category", serviceProvider.GetRequiredService<ProductControlCommandHandler>() },

                        { "/cart", serviceProvider.GetRequiredService<CartCommandHandler>() },
                        { "/cart_add", serviceProvider.GetRequiredService<CartCommandHandler>() },
                        { "/cart_remove", serviceProvider.GetRequiredService<CartCommandHandler>() },
                        { "/cart_page", serviceProvider.GetRequiredService<CartCommandHandler>() },

                        { "/address_create", serviceProvider.GetRequiredService<AddressControlCommandHandler>() },
                        { "/address_create_yes", serviceProvider.GetRequiredService<AddressControlCommandHandler>() },
                        { "/address_create_no", serviceProvider.GetRequiredService<AddressControlCommandHandler>() },
                        { "/address_create_ok", serviceProvider.GetRequiredService<AddressControlCommandHandler>() },
                        { "/address_create_cancel", serviceProvider.GetRequiredService<AddressControlCommandHandler>() }
                    }
                },
                {
                    UserRole.ServiceEngineer,
                    new Dictionary<string, ICommandHandler>
                    {
                        { "/start", serviceProvider.GetRequiredService<CommandHandlers.CommonCommandHandlers.StartCommandHandler>() },
                        { "/main_menu", serviceProvider.GetRequiredService<CommandHandlers.CommonCommandHandlers.StartCommandHandler>() },

                        { "/repairs", serviceProvider.GetRequiredService<RepairsCommandHandler>() },
                        { "/repair_page", serviceProvider.GetRequiredService<RepairsCommandHandler>() },
                        { "/repair_remove", serviceProvider.GetRequiredService<RepairsCommandHandler>() },
                        { "/repair_change_status", serviceProvider.GetRequiredService<RepairsCommandHandler>() },
                    }
                },
                {
                    UserRole.SalesManager,
                    new Dictionary<string, ICommandHandler>
                    {
                        { "/start", serviceProvider.GetRequiredService<CommandHandlers.CommonCommandHandlers.StartCommandHandler>() },
                        { "/main_menu", serviceProvider.GetRequiredService<CommandHandlers.CommonCommandHandlers.StartCommandHandler>() },

                        { "/orders", serviceProvider.GetRequiredService<OrdersCommandHandler>() },
                        { "/order_page", serviceProvider.GetRequiredService<OrdersCommandHandler>() },
                        { "/order_remove", serviceProvider.GetRequiredService<OrdersCommandHandler>() },
                        { "/order_change_status", serviceProvider.GetRequiredService<OrdersCommandHandler>() },
                    }
                }
            };
        }

        public async Task RouteCommandAsync(User user, MessageDTO message, string command)
        {
            var (baseCommand, opt1, opt2) = CommandParser.Parse(command);
            var args = new String[] {opt1, opt2};

            try
            {
                if (_roleHandlers.TryGetValue(user.Role, out var handlers) && handlers.TryGetValue(baseCommand, out var handler))
                {
                    await handler.CommandHandler(user, message, baseCommand, args);
                }
                else
                {
                    Console.WriteLine($"Неизвестная команда для роли {user.Role.GetDescription()}: {command}");
                    await _UI.SendMessageAsync(user.Id, $"Неизвестная команда для роли {user.Role.GetDescription()}: {command}");
                }
            } catch (Exception ex)
            {
                Console.WriteLine("Ошибка при обработке команды: " + ex.ToString());
                await _UI.SendMessageAsync(user.Id, $"Ухты что-то пошло не так.. Мы уже разбираемся!");
            }
        }
    }
}
