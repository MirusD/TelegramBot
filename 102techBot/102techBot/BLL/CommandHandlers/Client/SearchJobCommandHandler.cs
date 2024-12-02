using _102techBot.BLL.CommandHandlers.CommonCommandHandlers;
using _102techBot.BLL.Services.Users;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;

namespace _102techBot.BLL.CommandHandlers.Client
{
    /// <summary>
    /// Обработка комманд связанных с созданием заявки на обрытный звонок
    /// </summary>
    internal class SearchJobCommandHandler : BaseCommandHandler
    {
        private readonly StartCommandHandler _startCommandHandler;
        public SearchJobCommandHandler(
            IUI ui,
            ITemporaryStorageRepository temporaryStorageRepository,
            UserService userService,
            StartCommandHandler startCommandHandler) : base(ui, temporaryStorageRepository, userService)
        {
            _startCommandHandler = startCommandHandler;
        }

        public override async Task CommandHandler(User user, MessageDTO message, string command, string[] args)
        {
            switch (command)
            {
                case "/search_job_add_candidate":
                    await _userService.ChangeRoleAsync(user.Id, UserRole.Candidate);
                    await _UI.SendPushMessage(message.CallbackQueryId, "Ваша заявка отправлена, в ближайшее время мы ее расмотрим и свяжемся с вами");
                    await _startCommandHandler.CommandHandler(user, message, "/main_menu", args);
                    break;
            }
        }
    }
}
