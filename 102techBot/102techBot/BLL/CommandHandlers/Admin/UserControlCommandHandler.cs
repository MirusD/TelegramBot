using _102techBot.BLL.Services.Users;
using _102techBot.Common;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;
using _102techBot.Utils;

namespace _102techBot.BLL.CommandHandlers.Admin
{
    internal class UserControlCommandHandler : BaseCommandHandler
    {
        public UserControlCommandHandler(
            IUI ui,
            ITemporaryStorageRepository temporaryStorageRepository,
            UserService userService) : base(ui, temporaryStorageRepository, userService) 
        {
        }

        public override async Task CommandHandler(User user, MessageDTO message, string command, string[] args)
        {
            switch (command)
            {
                case "/users_control":
                    await _UI.EditMessageAsync(
                        user.Id,
                        message.Id,
                        $"> Управление пользователями:",
                        keyboard: Buttons.GetWorkWithUsersBtns()
                            .Concat(new[] { new Button { Text = "< Назад", Data = "/main_menu" } })
                            .ToList());
                    break;
                case "/users":
                    var selectUserGroup = args[0];

                    switch (selectUserGroup)
                    {
                        case "employes":
                            var employes = await _userService.GetUsersByGroupAsync(UserGroups.Employes);
                            if (employes.Count != 0)
                            {
                                await _UI.EditMessageAsync(user.Id, message.Id, $"> Сотрудники");
                                await PrintMessages(EntityFormatter.FormatUserInfo(employes), user, message);
                            }
                            else
                            {
                                await _UI.SendPushMessage(message.CallbackQueryId, "У вас нет сотрудников");
                            }
                            break;

                        case "candidates":
                            var candidates = await _userService.GetUsersByGroupAsync(UserGroups.Candidates);
                            if (candidates.Count != 0)
                            {
                                await _UI.SendMessageAsync(user.Id, message.Id, $"> Кандидаты");
                                await PrintMessages(EntityFormatter.FormatUserInfo(candidates), user, message);
                            }
                            else
                            {
                                await _UI.SendPushMessage(message.CallbackQueryId, "У вас нет кандидатов");
                            }
                            break;

                        case "clients":
                            var clients = await _userService.GetUsersByGroupAsync(UserGroups.Clients);
                            if (clients.Count != 0)
                            {
                                await _UI.SendMessageAsync(user.Id, message.Id, $"> Клиенты");
                                await PrintMessages(EntityFormatter.FormatUserInfo(clients), user, message);
                            }
                            else
                            {
                                await _UI.SendPushMessage(message.CallbackQueryId, "У вас нет клиентов");
                            }
                            break;
                    }
                    break;
                case "/role_change":
                    var selectUserId = args[0];

                    await _UI.EditMessageAsync(user.Id, message.Id, 
                        $"> Смена роли:", 
                        Buttons.GetRolesBtns(selectUserId)
                            .Concat(new[] { new Button { Text = "< Назад", Data = "/users_control" } })
                            .ToList());
                    break;

                case "/select_role":
                    var selectRole = args[0];
                    var userId = args[1];
                    if (long.TryParse(userId, out long id))
                    {
                        if (Enum.TryParse(selectRole, out UserRole role))
                        {
                            var changerRoleUser = await _userService.ChangeRoleAsync(id, role);
                            if (changerRoleUser != null)
                            {
                                await _UI.SendPushMessage(
                                    message.CallbackQueryId,
                                    $"== РОЛЬ ИЗМЕНЕНА ==\n{EntityFormatter.FormatUserInfo(changerRoleUser).Item2}"
                                );
                            }
                            else
                            {
                                await _UI.SendMessageAsync(
                                    user.Id, 
                                    $"Произошла ошибка, начните все сначала:",
                                    keyboard: Buttons.GetWorkWithUsersBtns()
                                        .Concat(new[] { new Button { Text = "< Назад", Data = "/main_menu" } })
                                        .ToList());
                            }
                        }
                    }
                    break;
            }
        }

        private async Task PrintMessages(Dictionary<long, string> dictUsersInfoCards, User user, MessageDTO message)
        {
            foreach (var userInfo in dictUsersInfoCards)
            {
                await _UI.SendMessageAsync(
                    user.Id,
                    message.Id,
                    userInfo.Value,
                    new List<Button>()
                    {
                                new Button
                                {
                                    Text = "Сменить роль",
                                    Data = $"/role_change/{userInfo.Key}",
                                    Ln = true
                                },
                                new Button
                                {
                                    Text = "Заблокировать",
                                    Data = $"/blocked/{userInfo.Key}",
                                    Ln = true
                                },
                                new Button
                                {
                                    Text = "< Назад",
                                    Data = $"/users_control",
                                    Ln = true
                                },
                    });
            }
        }
    }
}
