using _102techBot.BLL.Services.Users;
using _102techBot.Common;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;

namespace _102techBot.BLL.CommandHandlers.NewClient
{
    /// <summary>
    /// Обработка комманд связанных с началом работы пользователя с ботом
    /// </summary>
    internal class StartCommandHandler : BaseCommandHandler
    {
        public StartCommandHandler(
            IUI ui,
            ITemporaryStorageRepository temporaryStorageRepository,
            UserService userService) : base(ui, temporaryStorageRepository, userService) { }

        public override async Task CommandHandler(User user, MessageDTO message, string command, string[] args)
        {
            switch (user.State)
            {
                case UserState.None:
                    await _UI.SendMessageAsync(user.Id,
                        "Добрый день дорогой посетитель! " +
                        "Мы рады что вы здесь с нами. " +
                        "Очень хотим поскорее с вами познакомиться.");

                    try
                    {
                        if (user.FirstName != null)
                        {
                            await _UI.SendMessageAsync(
                                user.Id,
                                $"Мы можем к вам обращаться как {user.FirstName}?",
                                Buttons.GetConfirmBtns("confirm_name")
                            );
                        }
                        else
                        {
                            user = await _userService.ChangeStateAsync(user, UserState.AwaitingEnterName);
                            await _UI.SendMessageAsync(user.Id, "Напишите имя по которому мы к вам можем обращаться");
                        }
                    }
                    catch
                    {
                        await _UI.SendMessageAsync(
                            user.Id,
                            $"Произошла ошибка, повторите комманду /start"
                        );
                    }
                    break;
            }
        }
    }
}
