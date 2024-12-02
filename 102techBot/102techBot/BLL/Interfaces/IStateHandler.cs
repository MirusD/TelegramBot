using _102techBot.Domain.Entities;
using _102techBot.DTOs;

namespace _102techBot.BLL.Interfaces
{
    internal interface IStateHandler
    {
        Task HandleStateAsync(User user, MessageDTO message);
    }
}
