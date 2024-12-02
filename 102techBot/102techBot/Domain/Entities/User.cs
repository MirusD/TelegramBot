using LinqToDB.Mapping;
using System.ComponentModel;

namespace _102techBot.Domain.Entities
{
    enum UserState
    {
        //Common
        None,

        //New client
        AwaitingEnterName,

        //Client
        AwaitingRepairDescription,
        AwaitingRepairPhone,
        AwaitingCallbackPhone,
        AwaitingAddressCity,
        AwaitingAddressStreet,
        AwaitingAddressPostalCode,
        AwaitingAddressCountry,
        AwaitingAddressPhone,
        AwaitingAddressDescription,
        AwaitingAddressLatitude,
        AwaitingAddressLongitude,
        AwaitingAddressWorkingHours,

        //Admin
        AwaitingCategoryName,
        AwaitingProductName,
        AwaitingProductDescription,
        AwaitingProductPrice,
        AwaitingProductStock,
        AwaitingProductPhoto,

        Blocked,

        //ServiceEngineer
        WorkWithRepairRequests,
    }

    enum UserGroups
    {
        [Description("Сотрудники")]
        Employes,

        [Description("Кандиданты")]
        Candidates,

        [Description("Клиенты")]
        Clients
    }

    enum UserRole
    {
        [Description("Новый пользователь")]
        NewClient,

        [Description("Клиент")]
        Client,

        [Description("Администратор")]
        Administrator,

        [Description("Менеджер по продажам")]
        SalesManager,

        [Description("Сервис инжинер")]
        ServiceEngineer,

        [Description("Кандидат")]
        Candidate
    }

    [Table(Name = "Users")]
    internal class User
    {
        [PrimaryKey]
        public long Id { get; set; }

        [Column(Name = "UserName")]
        public string? UserName { get; set; }

        [Column(Name = "FirstName")]
        public string? FirstName { get; set; }

        [Column(Name = "LastName")]
        public string? LastName { get; set; }

        [Column(Name = "Email")]
        public string? Email { get; set; }

        [Column(Name = "Phone")]
        public string? Phone { get; set; }

        [Column(Name = "Role")]
        public UserRole Role { get; set; }

        [Column(Name = "State")]
        public UserState State { get; set; }
    }
}
