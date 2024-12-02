using _102techBot.BLL.Services.Carts;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq.Expressions;

namespace _102techBot.BLL.Services.Users
{
    internal class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly CartService _cartService;
        private readonly IUI _UI;
        public UserService(IUserRepository userRepository, CartService cartService, IUI ui)
        {
            _userRepository = userRepository;
            _cartService = cartService;
            _UI = ui;
        }

        public async Task<User> CheckAndGetActualUser(User user)
        {
            // TODO сделать кеширование
            var userWithDb = await _userRepository.GetByIdAsync(user.Id);

            if (userWithDb == null)
            {
                try
                {
                    var newUser = await _userRepository.AddAsync(user);
                    var cart = new Cart
                    {
                        UserId = newUser.Id,
                    };
                    await _cartService.CreateCart(cart);
                    return newUser;
                }
                catch (Exception ex)
                {
                    await _UI.SendMessageAsync(user.Id, "Что-то пошло не так. Попробуйте повторить команду");
                    throw;
                }
            }
            else
            {
                return userWithDb;
            }
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<List<User>> GetUsersByGroupAsync(UserGroups group)
        {
            var roles = new HashSet<UserRole>();

            switch (group)
            {
                case UserGroups.Employes:
                    roles = new HashSet<UserRole>
                        {
                            UserRole.Administrator,
                            UserRole.SalesManager,
                            UserRole.ServiceEngineer,
                        };
                    break;
                case UserGroups.Candidates:
                    roles = new HashSet<UserRole>
                        {
                            UserRole.Candidate,
                        };
                    break;
                case UserGroups.Clients:
                    roles = new HashSet<UserRole>
                        {
                            UserRole.Client,
                        };
                    break;

            }

            Expression<Func<User, bool>> roleEmployesFilter = u => roles.Contains(u.Role);

            return await _userRepository.GetUsersByRoleAsync(roleEmployesFilter);
        }

        public async Task<User?> ChangeRoleAsync(long userId, UserRole role)
        {
            try
            {
                var user = await GetUserById(userId);
                if (user == null) return null;

                user.Role = role;
                user.State = UserState.None;

                return await _userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при изменении роли пользователя {ex.Message}");
                throw;
            }
        }

        public async Task<User> ChangeStateAsync(User user, UserState state)
        {
            try
            {
                user.State = state;
                return await _userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при изменении состояния пользователя: {ex.Message}");
                throw;
            }
        }

        public async Task<User?> GetUserById(long userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<User> UpdateUser(User user)
        {
            //TODO сделать проверку и обновлять только если данные изменились
            try
            {
                return await _userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении данных пользователя в базе данных: {user} {ex.Message}");
                throw;
            }
        }
        //        private readonly UserUI _UI;
        //        private readonly IUserRepository _userRepository;
        //        private readonly ICartRepository _cartRepository;
        //        private readonly ClientService _clientService;
        //        private readonly AdminSevice _adminService;
        //        private readonly ServiceEngineerService _serviceEngineerService;
        //        public UserService(
        //            IUserRepository userRepository, 
        //            ICartRepository cartRepository,
        //            ClientService clientService, 
        //            AdminSevice adminService, 
        //            ServiceEngineerService serviceEngineerService,
        //            UserUI userUI) 
        //        {
        //            _userRepository = userRepository;
        //            _cartRepository = cartRepository;
        //            _clientService = clientService;
        //            _adminService = adminService;
        //            _serviceEngineerService = serviceEngineerService;
        //            _UI = userUI;
        //            _UI.HandlersOnMessage += HandleCommand;
        //            _UI.HandlersOnUpdate += HandleCommand;
        //        }
        //        public async Task HandleCommand(MessageDTO messageDTO)
        //        {
        //            var user = await _userRepository.GetByIdAsync(messageDTO.UserId);
        //            if (user == null)
        //            {
        //                user = new User()
        //                {
        //                    Id = messageDTO.UserId,
        //                    UserName = messageDTO.UserName,
        //                    FirstName = messageDTO.FirstName,
        //                    LastName = messageDTO.LastName,
        //                    Email = "",
        //                    Role = UserRole.Client,
        //                    State = UserState.None,
        //                };

        //                var cart = new Cart 
        //                {
        //                    UserId = user.Id,
        //                };

        //                await _userRepository.AddAsync(user);
        //                await _cartRepository.AddAsync(cart);
        //            }

        //            switch (user.Role)
        //            {
        //                case UserRole.Client:
        //                case UserRole.Candidate:
        //                    await _clientService.HandleCommand(user, messageDTO, messageDTO.Text);
        //                    break;
        //                case UserRole.Administrator:
        //                    await _adminService.HandleCommand(user, messageDTO, messageDTO.Text);
        //                    break;
        //                case UserRole.ServiceEngineer:
        //                    await _serviceEngineerService.HandleCommand(user, messageDTO, messageDTO.Text);
        //                    break;
        //                case UserRole.SalesManager:
        //                    break;
        //            }

        //            await _userRepository.UpdateAsync(user);
        //        }
    }
}
