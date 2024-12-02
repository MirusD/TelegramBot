using _102techBot.BLL.Interfaces;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using System.Linq.Expressions;

namespace _102techBot.BLL.Services
{
    internal class NotificationService : INotificationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUI _UI;
        public NotificationService(IUserRepository userRepository, IUI uI)
        {
            _userRepository = userRepository;
            _UI = uI;
        }

        public async Task NotifyUsersByRoleAsync(UserRole role, string message)
        {
            try
            {
                Expression<Func<User, bool>> roleFilter = u => u.Role == role;
                var users = await _userRepository.GetUsersByRoleAsync(roleFilter);

                foreach (var user in users)
                {
                    await _UI.SendMessageAsync(user.Id, message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка оповещения пользователей" + ex.ToString());
            }
        }

        public async Task NotifyUsersByRolesAsync(List<UserRole> roles, string message)
        {
            foreach (UserRole role in roles)
            {
                await NotifyUsersByRoleAsync(role, message);
            }
        }
    }
}
