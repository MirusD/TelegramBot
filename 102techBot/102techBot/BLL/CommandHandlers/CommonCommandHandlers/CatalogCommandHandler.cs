using _102techBot.BLL.Services.Categories;
using _102techBot.BLL.Services.Products;
using _102techBot.BLL.Services.Users;
using _102techBot.Common;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;
using _102techBot.Utils;

namespace _102techBot.BLL.CommandHandlers.CommonCommandHandlers
{
    /// <summary>
    /// Обработка комманд связанных с каталогом товаров
    /// </summary>
    internal class CatalogCommandHandler : BaseCommandHandler
    {
        private readonly CategoryService _categoryService;
        private readonly ProductService _productService;
        public CatalogCommandHandler(
            IUI ui,
            ITemporaryStorageRepository temporaryStorageRepository,
            CategoryService categoryService,
            ProductService productService,
            UserService userService) : base(ui, temporaryStorageRepository, userService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        public override async Task CommandHandler(User user, MessageDTO message, string command, string[] args)
        {
            switch (command)
            {
                case "/catalog":
                    await _UI.DeleteMessagesInBatchesAsync(user.Id);
                    var categories = await _categoryService.GetCategoriesAsync();

                    if (categories.Count != 0)
                    {
                        await _UI.EditMessageAsync(
                            user.Id,
                            message.Id,
                            $"Каталог товаров",
                            keyboard: Buttons.GetCategoryBtns(categories)
                                .Concat(new[] { new Button { Text = "< Назад", Data = "/main_menu" } })
                                .ToList());
                    }
                    else
                    {
                        await _UI.SendPushMessage(message.CallbackQueryId, "В каталоге пока нет товаров");
                    }
                    break;
                case "/category":
                    var selectCategoryId = args[0];
                    if (int.TryParse(selectCategoryId, out int categoryId))
                    {
                        var products = await _productService.GetProductsByCategoryAsync(categoryId);
                        _storageRepository.Add(user.Id.ToString(), products);

                        if (products.Count != 0)
                        {
                            await SendPaginationMessage(user, message, 1);
                        }
                        else
                        {
                            await _UI.SendPushMessage(message.CallbackQueryId, $"Товаров в данной категории пока нет");
                        }
                    }
                    break;
                case "/page":
                    var page = args[0];
                    await SendPaginationMessage(user, message, int.Parse(page));
                    break;
            }
        }

        private async Task SendPaginationMessage(User user, MessageDTO message, int page)
        {
            var tempProducts = _storageRepository.Get<List<Product>>(user.Id.ToString());

            if (tempProducts != null)
            {
                await _UI.RemoveMessageAsync(user.Id, message.Id);
                await _UI.SendPhotoPaginatedMessage(
                    user.Id,
                    message.Id,
                    page,
                    tempProducts,
                    product => $"{EntityFormatter.FormatProductInfo(product)}",
                    product => Path.Combine(AppContext.BaseDirectory, "Img", product?.ImageUrl == null ? "" : product.ImageUrl),
                    product => new List<Button>
                        {
                        new Button
                            {
                                Text = "В корзину",
                                Data = $"/cart_add/{product.Id}"
                            }
                        },
                    page => {
                        var navBtns = new List<Button>
                        {
                            new Button
                            {
                                Text = "< Назад в меню",
                                Data = "/catalog",
                                Ln = true
                            }
                        };

                        if (page < (int)Math.Ceiling(tempProducts.Count / (double)3))
                        {
                            navBtns.Add(new Button
                            {
                                Text = "⬇️ Смотреть еще ⬇️",
                                Data = $"/cart_page/{page + 1}",
                                Ln = true
                            }); ;
                        }

                        return navBtns;
                    },
                    itemsPerPage: 3);
            }
        }
    }
}
