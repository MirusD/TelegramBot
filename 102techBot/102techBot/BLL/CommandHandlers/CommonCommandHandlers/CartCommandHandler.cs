using _102techBot.BLL.Routers;
using _102techBot.BLL.Services.Carts;
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
    /// Обработка комманд связанных с корзиной
    /// </summary>
    internal class CartCommandHandler : BaseCommandHandler
    {
        private readonly CartService _cartService;
        public CartCommandHandler(
            IUI ui,
            ITemporaryStorageRepository temporaryStorageRepository,
            UserService userService,
            CartService cartService) : base(ui, temporaryStorageRepository, userService)
        {
            _cartService = cartService;
        }

        public override async Task CommandHandler(User user, MessageDTO message, string command, string[] args)
        {
            switch (command)
            {
                case "/cart":
                    var cartItems = await _cartService.GetCartItems(user.Id);
                    if (cartItems != null)
                    {
                        if (cartItems.Count != 0)
                        {
                            _storageRepository.Add(user.Id.ToString(), cartItems);
                            await SendPaginationMessage(user, message, 1);
                        }
                        else
                        {
                            await _UI.SendPushMessage(
                                message.CallbackQueryId,
                                "Корзина пуста, начните добавлять товары в корзину и здесь будет на что посмотреть"
                            );
                        }
                    }
                    else
                        await _UI.SendPushMessage(
                            message.CallbackQueryId, 
                            "Что-то пошло не так, попробуйте по новой открыть корзину"
                        );
                    break;
                case "/cart_add":
                    var selectProductId = args[0];
                    if (long.TryParse(selectProductId, out long id))
                    {
                        await _UI.RemoveBtnsAsync(user.Id, message.Id);
                        await _cartService.AddProductAsync(user.Id, id);
                    }
                    break;
                case "/cart_page":
                    var page = args[0];
                    await SendPaginationMessage(user, message, int.Parse(page));
                    break;
                case "/cart_remove":
                    var removeCartItemId = args[0];
                    if (long.TryParse(removeCartItemId, out long removeItemId))
                    {
                        var removeItem = await _cartService.RemoveCartItem(removeItemId);
                        if (removeItem != null)
                        {
                            await _UI.RemoveMessageAsync(user.Id, message.Id);
                        }
                        else
                        {
                            await _UI.SendPushMessage(
                                message.CallbackQueryId, 
                                "Что-то пошло не так, не удалось удалить товар из корзины. Попробуйте еще раз."
                            );
                        }
                    }
                    break;
            }
        }

        private async Task SendPaginationMessage(User user, MessageDTO message, int page)
        {
            var tempCartITems = _storageRepository.Get<List<CartItem>>(user.Id.ToString());

            if (tempCartITems != null)
            {
                await _UI.RemoveMessageAsync(user.Id, message.Id);
                await _UI.SendPhotoPaginatedMessage(
                    user.Id,
                    message.Id,
                    page,
                    tempCartITems,
                    cartItem => $"{EntityFormatter.FormatProductInfo(cartItem.Product)}",
                    cartItem => Path.Combine(AppContext.BaseDirectory, "Img", cartItem.Product?.ImageUrl == null ? "" : cartItem.Product.ImageUrl),
                    cartItem => new List<Button>
                        {
                            new Button 
                            {
                                Text = "Удалить",
                                Data = $"/cart_remove/{cartItem.Id}",
                                Ln = true
                            }
                        },
                    page => {
                        var btns = new List<Button>
                        {
                            new Button
                            {
                                Text = "Оформить заказ",
                                Data = $"/order_add",
                                Ln = true
                            },
                            new Button
                            {
                                Text = "< Назад",
                                Data = $"/main_menu",
                                Ln = true
                            }
                        };

                        if (page < (int)Math.Ceiling(tempCartITems.Count / (double)3))
                        {
                            btns.Add(new Button
                            {
                                Text = "⬇️ Смотреть еще ⬇️",
                                Data = $"/cart_page/{page + 1}",
                                Ln = true
                            }); ;
                        }

                        return btns;
                    },     
                    itemsPerPage: 3);
            }
        }
    }
}
