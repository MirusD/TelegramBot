using _102techBot.Domain.Entities;
using _102techBot.DTOs;

namespace _102techBot.BLL.Interfaces
{
    internal interface ICommandHandler
    {
        Task CommandHandler(User user, MessageDTO message, string command, string[] args);
    }
}
