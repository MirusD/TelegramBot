using _102techBot.BLL.Interfaces;
using _102techBot.BLL.Routers;
using _102techBot.BLL.Services.Carts;
using _102techBot.BLL.Services.Categories;
using _102techBot.BLL.Services.Products;
using _102techBot.BLL.Services.Repairs;
using _102techBot.BLL.Services.Users;
using _102techBot.Common;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;
using _102techBot.Utils;
using AutoMapper;

namespace _102techBot.BLL.CommandHandlers.Client
{
    /// <summary>
    /// Обработка комманд связанных с созданием заявки на ремонт
    /// </summary>
    internal class RepairCommandHandler : BaseCommandHandler
    {
        private readonly CategoryService _categoryService;
        private readonly ProductService _productService;
        private readonly CartService _cartService;
        private readonly RepairService _repairService;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        public RepairCommandHandler(
            IUI ui,
            ITemporaryStorageRepository temporaryStorageRepository,
            CategoryService categoryService,
            ProductService productService,
            UserService userService,
            CartService cartService,
            RepairService repairService,
            INotificationService notificationService,
            IMapper mapper) : base(ui, temporaryStorageRepository, userService)
        {
            _categoryService = categoryService;
            _productService = productService;
            _cartService = cartService;
            _repairService = repairService;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public override async Task CommandHandler(User user, MessageDTO message, string command, string[] args)
        {
            switch (command)
            {
                case "/repair_create":
                    var categories = await _categoryService.GetCategoriesAsync();
                    if (categories.Count == 0)
                    {
                        await _UI.SendPushMessage(
                            message.CallbackQueryId,
                            "Пока нет категории, к сожалению вы не можете продолжить создание заявки...");
                    }
                    else
                    {
                        await _UI.EditMessageAsync(user.Id, message.Id, "Начало создания заявки на ремонт");
                        await _UI.SendMessageAsync(
                            user.Id,
                            "Выберите тип оборудования:",
                            Buttons.GetCategoryBtns("/repair_create", categories));
                    }
                    break;
                case "/repair_create_select_category":
                    var selectCategoryId = args[0];
                    if (long.TryParse(selectCategoryId, out long categoryId))
                    {
                        var products = await _productService.GetProductsByCategoryAsync(categoryId);
                        var category = await _categoryService.GetCategoryByIdAsync(categoryId);

                        var tempRepair = new TemporaryRepair
                        {
                            CategoryId = category.Id,
                            Category = category,
                            UserId = user.Id,
                            User = user
                        };

                        _storageRepository.Add(user.Id.ToString(), tempRepair);

                        if (products.Count == 0)
                        {
                            await _UI.SendPushMessage(
                                message.CallbackQueryId,
                                "Пока нет моделей, к сожалению вы не можете продолжить оформление заявки...");
                        }
                        else
                        {
                            await _UI.EditMessageAsync(user.Id, message.Id, $"Вы выбрали тип оборудования: \"{category?.Name}\"");
                            await _UI.SendMessageAsync(
                                user.Id,
                                "Выберите модель:",
                                Buttons.GetProductBtns("/repair_create", products));
                        }
                    }
                    break;
                case "/repair_create_select_model":
                    var selectModelId = args[0];
                    if (int.TryParse(selectModelId, out int productId))
                    {
                        var product = await _productService.GetProductByIdAsync(productId);
                        var tr1 = _storageRepository.Get<TemporaryRepair>(user.Id.ToString());
                        if (tr1 != null && product != null)
                        {
                            tr1.ProductId = product.Id;
                            tr1.Product = product;
                        }
                        else 
                        {
                            await _UI.SendMessageAsync(user.Id, $"Возникала ошибка, попробуйте повторить /repair_create");
                        }
                        _storageRepository.Add(user.Id.ToString(), tr1);

                        await _userService.ChangeStateAsync(user, UserState.AwaitingRepairDescription);
                        await _UI.EditMessageAsync(user.Id, message.Id, $"Вы выбрали модель: \"{product?.Name}\"");
                        await _UI.SendMessageAsync(user.Id, "Опишите проблему:");
                    }
                    break;
                case "/repair_send":
                    var tmpRepair = _storageRepository.Get<TemporaryRepair>(user.Id.ToString());
                    var createdRepairRequest = _mapper.Map<Repair>(tmpRepair);
                    var newRepair = await _repairService.AddRepairAsync(createdRepairRequest);
                    if (newRepair != null)
                    {
                        await _notificationService.NotifyUsersByRolesAsync(
                            new List<UserRole> { UserRole.ServiceEngineer, UserRole.Administrator },
                            "Добавлена новая заявка на ремонт"
                        );
                        await _UI.EditMessageAsync(
                            user.Id,
                            message.Id,
                            "Заявка отправлена",
                            new List<Button> { new Button { Text = "< Назад в меню", Data = "/main_menu" } }
                        );
                    }
                    break;
                case "/repair_cancel":
                    _storageRepository.Remove(user.Id.ToString());
                    await _UI.EditMessageAsync(
                        user.Id, 
                        message.Id,
                        "Отправка заявки отменена", 
                        new List<Button> { new Button { Text = "< Назад в меню", Data = "/main_menu"} }
                    );
                    break;
            }
        }

        private async Task SendPaginationMessage(User user, MessageDTO message, int page)
        {
            var tempRepairs = _storageRepository.Get<List<Repair>>(user.Id.ToString());

            if (tempRepairs != null)
            {
                await _UI.SendPaginatedMessage(
                    user.Id,
                    message.Id,
                    1,
                    tempRepairs,
                    repair => $"Заявка:\n{EntityFormatter.FormaRepairInfo(repair)}",
                    page =>
                    {
                        var repair = tempRepairs[page - 1];
                        var (buttonText, command) = GetButtonDetailsForRepairStatus(repair.Id, repair.Status);

                        var btns = new List<Button>()
                        {
                                                new Button {
                                                    Text = buttonText,
                                                    Data = command,
                                                    Ln = true
                                                },
                                                new Button {
                                                    Text = "< Назад в меню",
                                                    Data = "/back",
                                                    Ln = true
                                                }
                        };
                        return btns;
                    });
            }
        }

        private (string buttonText, string command) GetButtonDetailsForRepairStatus(long repairId, RepairStatus status)
        {
            return status switch
            {
                RepairStatus.Created => ("Подтвердить", $"/repair_change_status/{repairId}/{(int)RepairStatus.Confirmed}"),
                RepairStatus.Confirmed => ("Взять в работу", $"/repair_change_status/{repairId}/{(int)RepairStatus.AtWork}"),
                RepairStatus.AtWork => ("Завершить", $"/repair_change_status/{repairId}/{(int)RepairStatus.Completed}"),
                RepairStatus.Completed => ("Закрыть", $"/repair_remove/{repairId}"),
                _ => (string.Empty, string.Empty) // Если статус не обрабатывается
            };
        }
    }
}
