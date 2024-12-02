using _102techBot.BLL.Services.Users;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;
using AutoMapper;

namespace _102techBot.BLL.StateHandlers.Admin
{
    /// <summary>
    /// Обработка состояния ожидания ввода названия товара
    /// </summary>
    internal class AwaitingProductNameStateHandler : BaseStateHandler
    {
        private readonly UserService _userService;
        public AwaitingProductNameStateHandler(
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
                var newProduct = _storageRepository.Get<Product>(user.Id.ToString());
                if (newProduct != null)
                {
                    newProduct.Name = message.Text;
                }

                _storageRepository.Add(user.Id.ToString(), newProduct);

                await _userService.ChangeStateAsync(user, UserState.AwaitingProductDescription);
                await _UI.SendMessageAsync(user.Id, "Опишите товар:");
            }
            else
            {
                await _UI.SendPushMessage(message.CallbackQueryId, "Вы отправили пустую строку, введите название товара");
            }
        }
    }
}
