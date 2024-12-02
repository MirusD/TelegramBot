using _102techBot.BLL.Interfaces;
using _102techBot.BLL.Routers;
using _102techBot.BLL.Services.Users;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;
using _102techBot.Infrastructure.DAL;

namespace _102techBot.BLL.CommandHandlers
{
    internal abstract class BaseCommandHandler : ICommandHandler
    {
        protected readonly IUI _UI;
        protected readonly ITemporaryStorageRepository _storageRepository;
        protected readonly UserService _userService;

        public BaseCommandHandler(
            IUI ui, 
            ITemporaryStorageRepository temporaryStorageRepository,
            UserService userService)
        {
            _UI = ui;
            _storageRepository = temporaryStorageRepository;
            _userService = userService;
        }

        public abstract Task CommandHandler(User user, MessageDTO message, string command, string[] args);
    }
}
