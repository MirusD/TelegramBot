using _102techBot.BLL.Services.Products;
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
    /// Обработка состояния ожидания отправки фотографии товара
    /// </summary>
    internal class AwaitingProductPhotoStateHandler : BaseStateHandler
    {
        private readonly UserService _userService;
        private readonly ProductService _productService;
        public AwaitingProductPhotoStateHandler(
            IUI ui,
            ITemporaryStorageRepository temporaryStorageRepository,
            IMapper mapper,
            UserService userService,
            ProductService productService) : base(ui, temporaryStorageRepository, mapper) 
        {
            _userService = userService;
            _productService = productService;
        }

        public override async Task HandleStateAsync(User user, MessageDTO message)
        {
            if (message.PhotoFileId != null)
            {
                var fileName = await _UI.GetPhoto(message.PhotoFileId);
                var newProduct = _storageRepository.Get<Product>(user.Id.ToString());
                if (newProduct != null)
                {
                    newProduct.ImageUrl = fileName;
                    newProduct.Status = ProductStatus.Active;
                    await _productService.CreateProductAsync(newProduct);
                }

                _storageRepository.Remove(user.Id.ToString());
                await _userService.ChangeStateAsync(user, UserState.None);
                await _UI.SendMessageAsync(
                    user.Id,
                    message.Id,
                    "Товар добавлен!",
                    new List<Button> {
                        new Button { Text = "Добавить еще", Data = "/add_product" },
                        new Button { Text = "Назад", Data = "/products_control" }
                    });
            }
            else
            {
                await _UI.SendPushMessage(message.CallbackQueryId, "Что-то пошло не так, пожалуйста отправьте заново фотографию товара");
            }
        }
    }
}
