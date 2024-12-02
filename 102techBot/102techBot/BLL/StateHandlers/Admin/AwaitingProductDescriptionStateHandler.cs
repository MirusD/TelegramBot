using _102techBot.BLL.Services.Users;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;
using AutoMapper;

namespace _102techBot.BLL.StateHandlers.Admin
{
    /// <summary>
    /// Обработка состояния ожидания ввода описания товара
    /// </summary>
    internal class AwaitingProductDescriptionStateHandler : BaseStateHandler
    {
        private readonly UserService _userService;
        public AwaitingProductDescriptionStateHandler(
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
                    newProduct.Description = message.Text;
                }

                _storageRepository.Add(user.Id.ToString(), newProduct);

                await _userService.ChangeStateAsync(user, UserState.AwaitingProductPrice);
                await _UI.SendMessageAsync(user.Id, "Цена за единицу:");
            }
            else
            {
                await _UI.SendPushMessage(message.CallbackQueryId, "Вы отправили пустую строку, пожалуйста опишите товар");
            }
        }
    }
}
