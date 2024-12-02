using _102techBot.BLL.Services.Users;
using _102techBot.Common;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;

namespace _102techBot.BLL.CommandHandlers.CommonCommandHandlers
{
    /// <summary>
    /// Обработка комманд связанных с началом работы ботка
    /// </summary>
    internal class StartCommandHandler : BaseCommandHandler
    {
        public StartCommandHandler(
            IUI ui,
            ITemporaryStorageRepository temporaryStorageRepository,
            UserService userService) : base(ui, temporaryStorageRepository, userService) { }

        public override async Task CommandHandler(User user, MessageDTO message, string command, string[] args)
        {
            switch (command)
            {
                case "/start":
                    await _UI.SendMessageAsync(
                        user.Id,
                        message.Id,
                        $"Добрый день {user.FirstName}! " +
                        "Мы рады что вы вернулись к нам!",
                        keyboard: Buttons.GetMainMenuBtns(user)
                        );
                    break;
                case "/main_menu":
                    await _UI.DeleteMessagesInBatchesAsync(user.Id);
                    await _UI.EditMessageAsync(
                        user.Id,
                        message.Id,
                        "> Главное меню",
                        keyboard: Buttons.GetMainMenuBtns(user)
                        );
                    break;
            }
        }
    }
}
