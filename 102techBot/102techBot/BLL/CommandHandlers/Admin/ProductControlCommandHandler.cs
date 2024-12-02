using _102techBot.BLL.Services.Categories;
using _102techBot.BLL.Services.Products;
using _102techBot.BLL.Services.Users;
using _102techBot.Common;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;
using _102techBot.Infrastructure.DAL;
using _102techBot.Utils;

namespace _102techBot.BLL.CommandHandlers.Admin
{
    internal class ProductControlCommandHandler : BaseCommandHandler
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        public ProductControlCommandHandler(
            IUI ui,
            ITemporaryStorageRepository temporaryStorageRepository,
            ProductService productService,
            CategoryService categoryService,
            UserService userService) : base(ui, temporaryStorageRepository, userService) 
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public override async Task CommandHandler(User user, MessageDTO message, string command, string[] args)
        {
            switch (command)
            {
                case "/products_control":
                    await _UI.EditMessageAsync(
                        user.Id,
                        message.Id,
                        $"> Управление товарами:",
                        keyboard: Buttons.GetWorkProductsBtns()
                            .Concat(new[] { new Button { Text = "< Назад", Data = "/main_menu" } })
                            .ToList());
                    break;
                case "/add_category":
                    try
                    {
                        await _userService.ChangeStateAsync(user, UserState.AwaitingCategoryName);
                        await _UI.EditMessageAsync(user.Id, message.Id, "Введите название новой категории товаров:");
                    } 
                    catch
                    {
                        await _UI.EditMessageAsync(user.Id, message.Id, "Произошла ошибка, повторите комманду \"/add_category\"");
                    }
                    break;
                case "/add_product":
                    var categories = await _categoryService.GetCategoriesAsync();
                    if (categories.Count != 0)
                    {
                        await _UI.EditMessageAsync(
                            user.Id,
                            message.Id,
                            "К какой категории относиться товар?",
                            Buttons.GetCategoryBtns("/add_product", categories)
                                .Concat(new[] { new Button { Text = "< Назад", Data = "/products_control" } })
                                .ToList());
                    }
                    else
                    {
                        await _UI.SendPushMessage(message.CallbackQueryId, "Нет категории, для начала создайте категории");
                    }
                    break;
                case "/add_product_select_category":
                    var categoryId = args[0];
                    if (long.TryParse(categoryId, out long id))
                    {
                        var newProduct = new Product
                        {
                            CategoryId = id,
                        };

                        _storageRepository.Add(user.Id.ToString(), newProduct);

                        await _userService.ChangeStateAsync(user, UserState.AwaitingProductName);
                        await _UI.EditMessageAsync(user.Id, message.Id, "Введите название товара:");
                    }
                    break;
            }
        }
    }
}
