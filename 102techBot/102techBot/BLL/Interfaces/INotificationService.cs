using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _102techBot.Domain.Entities;

namespace _102techBot.BLL.Interfaces
{
    internal interface INotificationService
    {
        public Task NotifyUsersByRoleAsync(UserRole role, string message);
        public Task NotifyUsersByRolesAsync(List<UserRole> roles, string message);
    }
}
