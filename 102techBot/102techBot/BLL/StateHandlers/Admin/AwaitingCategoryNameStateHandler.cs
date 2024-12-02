using _102techBot.BLL.Services.Categories;
using _102techBot.BLL.Services.Users;
using _102techBot.Common;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;
using AutoMapper;

namespace _102techBot.BLL.StateHandlers.Admin
{
    /// <summary>
    /// Обработка состояния ожидания ввода названия категории
    /// </summary>
    internal class AwaitingCategoryNameStateHandler : BaseStateHandler
    {
        private readonly CategoryService _categoryService;
        private readonly UserService _userService;
        public AwaitingCategoryNameStateHandler(
            IUI ui,
            ITemporaryStorageRepository temporaryStorageRepository,
            IMapper mapper,
            CategoryService categoryService,
            UserService userService) : base(ui, temporaryStorageRepository, mapper) 
        {
            _categoryService = categoryService;
            _userService = userService;
        }

        public override async Task HandleStateAsync(User user, MessageDTO message)
        {
            if (!string.IsNullOrEmpty(message.Text))
            {
                var newCategory = await _categoryService.AddNewCategoryAsync(message.Text);
                await _UI.SendMessageAsync(
                    user.Id,
                    $"Категория '{newCategory.Name}' создана",
                    new List<Button> {
                        new Button { Text = "Добавить еще", Data = "/add_category" },
                        new Button { Text = "Назад", Data = "/products_control" }
                    });

                await _userService.ChangeStateAsync(user, UserState.None);
            }
            else
            {
                await _UI.SendPushMessage(message.CallbackQueryId, "Вы отправили пустую строку, пожалуйста введите название категории");
            }
        }
    }
}
