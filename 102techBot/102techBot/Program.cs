using _102techBot.Domain;
using _102techBot.Infrastructure.UI;
using _102techBot.Infrastructure.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using LinqToDB.Data;
using Microsoft.Extensions.DependencyInjection;
using LinqToDB;
using _102techBot.BLL.Routers;
using _102techBot.Domain.Entities;
using _102techBot.BLL.CommandHandlers.NewClient;
using _102techBot.BLL.StateHandlers.NewClient;
using _102techBot.BLL.Services.Categories;
using _102techBot.BLL.CommandHandlers.CommonCommandHandlers;
using _102techBot.BLL.CommandHandlers.Admin;
using _102techBot.BLL.StateHandlers.Admin;
using _102techBot.BLL.Services.Products;
using _102techBot.BLL.Services.Carts;
using _102techBot.BLL.CommandHandlers.Client;
using _102techBot.BLL.Services.Repairs;
using _102techBot.BLL.StateHandlers.Client;
using _102techBot.BLL.Services.Users;
using _102techBot.Domain.Repositories;
using _102techBot.BLL.Services.Callbacks;
using _102techBot.BLL.StateHandlers.WorkWithAddress.Admin;
using _102techBot.BLL.Services.Addresses;
using _102techBot.BLL.Services.Orders;
using _102techBot.BLL.Services;
using _102techBot.BLL.Interfaces;
using _102techBot.BLL.CommandHandlers.ServiceEngineer;
using _102techBot.BLL.CommandHandlers.SalesManager;

namespace _102techBot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddUserSecrets<Program>();
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddScoped<AppDbContext>(provider => new AppDbContext(connectionString));
            builder.Services.AddScoped<DatabaseInitializer>();
            builder.Services.AddSingleton<IUI, TelegramService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRepairRepository, RepairRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            builder.Services.AddScoped<ICallbackRepository, CallbackRepository>();
            builder.Services.AddScoped<IAddressRepository, AddressRepository>();
            builder.Services.AddSingleton<ITemporaryStorageRepository, TemporaryStorageRepository>();

            //Routers
            builder.Services.AddScoped<StateRouter>();
            builder.Services.AddScoped<CommandRouter>();

            //NewClient
            //CommandHandlers
            builder.Services.AddScoped<BLL.CommandHandlers.NewClient.StartCommandHandler>();
            builder.Services.AddScoped<ConfirmNameCommandHandler>();
            //StatesHandlers
            builder.Services.AddScoped<AwaitingNameStateHandler>();

            //Client
            //CommandHandlers
            builder.Services.AddScoped<RepairCommandHandler>();
            builder.Services.AddScoped<CallbackCommandHandler>();
            builder.Services.AddScoped<SearchJobCommandHandler>();
            builder.Services.AddScoped<ShopsAndServicesCommandHandler>();
            //StateHandlers
            builder.Services.AddScoped<AwaitingRepairDescriptionStateHandler>();
            builder.Services.AddScoped<AwaitingRepairPhoneStateHandler>();
            builder.Services.AddScoped<AwaitingCallbackPhoneStateHandler>();


            //Admin
            //CommandHandlers
            builder.Services.AddScoped<UserControlCommandHandler>();
            builder.Services.AddScoped<ProductControlCommandHandler>();
            builder.Services.AddScoped<AddressControlCommandHandler>();
            //StatesHandlers
            builder.Services.AddScoped<AwaitingCategoryNameStateHandler>();
            builder.Services.AddScoped<AwaitingProductNameStateHandler>();
            builder.Services.AddScoped<AwaitingProductDescriptionStateHandler>();;
            builder.Services.AddScoped<AwaitingProductPriceStateHandler>();
            builder.Services.AddScoped<AwaitingProductPhotoStateHandler>();
            builder.Services.AddScoped<AwaitingProductStockStateHandler>();

            builder.Services.AddScoped<AwaitingAddressCountryStateHandler>();
            builder.Services.AddScoped<AwaitingAddressCityStateHandler>();
            builder.Services.AddScoped<AwaitingAddressStreetStateHandler>();
            builder.Services.AddScoped<AwaitingAddressLongitudeStateHandler>();
            builder.Services.AddScoped<AwaitingAddressLatitudeStateHandler>();
            builder.Services.AddScoped<AwaitingAddressPhoneStateHandler>();
            builder.Services.AddScoped<AwaitingAddressPostalCodeStateHandler>();
            builder.Services.AddScoped<AwaitingAddressWorkingHoursStateHandler>();

            //ServiceEngineer
            builder.Services.AddScoped<RepairsCommandHandler>();

            //SalesManager
            builder.Services.AddScoped<OrdersCommandHandler>();

            //Common
            builder.Services.AddScoped<CatalogCommandHandler>();
            builder.Services.AddScoped<BLL.CommandHandlers.CommonCommandHandlers.StartCommandHandler>();
            builder.Services.AddScoped<CartCommandHandler>();
            builder.Services.AddScoped<OrderCommandHandler>();            

            //Services
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<CartService>();
            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<RepairService>();
            builder.Services.AddScoped<CallbackService>();
            builder.Services.AddScoped<AddressService>();
            builder.Services.AddScoped<OrderService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var app = builder.Build();

            try
            {
                // Инициализация базы данных
                var dbInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
                await dbInitializer.CreateTablesIfNotExists();

                // Запуск роутера
                var stateRouter = app.Services.GetRequiredService<StateRouter>();

                // Поддерживаем приложение запущенным
                await Task.Delay(Timeout.Infinite);
            }
            catch (Exception ex)
            {
                // Логирование ошибок
                Console.WriteLine($"Ошибка при запуске приложения: {ex.Message}");
            }

            await Task.Delay(Timeout.Infinite);

        }

        public class AppDbContext : DataConnection
        {
            public AppDbContext(string connectionString) : base(ProviderName.PostgreSQL, connectionString) { }
            public ITable<User> Users => this.GetTable<User>();
            public ITable<Category> Categories => this.GetTable<Category>();
            public ITable<Product> Products => this.GetTable<Product>();
            public ITable<Cart> Carts => this.GetTable<Cart>();
            public ITable<CartItem> CartItems => this.GetTable<CartItem>();
            public ITable<Order> Orders => this.GetTable<Order>();
            public ITable<OrderItem> OrderItems => this.GetTable<OrderItem>();
            public ITable<Repair> Repairs => this.GetTable<Repair>();
            public ITable<Callback> Callbacks => this.GetTable<Callback>();
            public ITable<Address> Addresses => this.GetTable<Address>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}