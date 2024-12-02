using LinqToDB.Mapping;
using System.ComponentModel;

namespace _102techBot.Domain.Entities
{
    public enum OrderStatus
    {
        [Description("В процессе создания")]
        Creating,

        [Description("Создан")]
        Created,

        [Description("Подтверждён")]
        Confirmed,

        [Description("В работе")]
        AtWork,

        [Description("Завершон")]
        Completed
    }

    [Table(Name = "Orders")]
    internal class Order
    {
        [PrimaryKey, Identity]
        [Column(Name = "Id")]
        public long Id { get; set; }

        [Column(Name = "UserId")]
        public required long UserId { get; set; }

        [Association(ThisKey = "UserId", OtherKey = "Id")]
        public User User { get; set; }

        [Column(Name = "TotalAmount")]
        public required decimal TotalAmount { get; set; }

        [Column(Name = "Status")]
        public required OrderStatus Status { get; set; }
    }
}
