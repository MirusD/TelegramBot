using _102techBot.BLL.Services.Users;
using _102techBot.Common;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;

namespace _102techBot.BLL.CommandHandlers.NewClient
{
    /// <summary>
    /// Обработка комманда связанных с подтверждением имени пользователя
    /// </summary>
    internal class ConfirmNameCommandHandler : BaseCommandHandler
    {
        public ConfirmNameCommandHandler(
            IUI ui, 
            ITemporaryStorageRepository temporaryStorageRepository,
            UserService userService) : base(ui, temporaryStorageRepository, userService) {}

        public override async Task CommandHandler(User user, MessageDTO message, string command, string[] args)
        {
            try
            {
                if (command == "/confirm_name_yes")
                {
                    var tempFirstName = _storageRepository.Get<String>(user.Id.ToString());

                    if (tempFirstName != null)
                    {
                        user.FirstName = tempFirstName;
                        _storageRepository.Remove(user.Id.ToString());
                    }

                    user = await _userService.ChangeRoleAsync(user.Id, UserRole.Client) ?? user;
                    await _UI.EditMessageAsync(
                        user.Id,
                        message.Id,
                        $"{user.FirstName} очень приятно познакомиться! " +
                        $"Вы сделали нас чуточку счастливее! Теперь наша очередь радовать вас. " +
                        $"Мы занимаемся продажей и ремонтом оборудования сельхозназначения и вот " +
                        $"что у нас есть для вас:");
                    await _UI.SendMessageAsync(
                        user.Id,
                        message.Id,
                        "> Главное меню",
                        Buttons.GetMainMenuBtns(user)
                    );
                }
                else if (command == "/confirm_name_no")
                {
                    if (user.State != UserState.AwaitingEnterName)
                    {
                        await _userService.ChangeStateAsync(user, UserState.AwaitingEnterName);
                    }
                    _storageRepository.Remove(user.Id.ToString());
                    await _UI.SendMessageAsync(
                        user.Id,
                        message.Id,
                        "Тогда напишите имя по которому мы к вам можем обращаться"
                    );
                }
            } catch (Exception) 
            {
                await _UI.SendMessageAsync(
                    user.Id,
                    message.Id,
                    "Что-то пошло не так, попробуйте еще раз..."
                );
                await _UI.SendMessageAsync(
                    user.Id,
                    $"Мы можем к вам обращаться как {user.FirstName}?",
                    Buttons.GetConfirmBtns("confirm_name")
                );
            }
        }
    }
}
