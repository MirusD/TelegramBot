using LinqToDB.Mapping;
using System.ComponentModel;

namespace _102techBot.Domain.Entities
{
    public enum RepairStatus
    {
        [Description("Создаётся")]
        Creating,

        [Description("Создан")]
        Created,

        [Description("Подтверждён")]
        Confirmed,

        [Description("В работе")]
        AtWork,

        [Description("Завершен")]
        Completed
    }

    [Table(Name = "Repairs")]
    internal class Repair
    {
        [PrimaryKey, Identity]
        [Column(Name = "Id")]
        public long Id { get; set; }

        [Column(Name = "UserId")]
        public required long UserId { get; set; }

        [Association(ThisKey = "UserId", OtherKey = "Id")]
        public User? User { get; set; }

        [Column(Name = "ProductId")]
        public required long ProductId { get; set; }

        [Association(ThisKey = "ProductId", OtherKey = "Id")]
        public Product? Product { get; set; }

        [Column(Name = "Description")]
        public required string Description { get; set; }

        [Column(Name = "Phone")]
        public required string Phone { get; set; }

        [Column(Name = "Status")]
        public required RepairStatus Status { get; set; }

        [Column(Name = "CategoryId")]
        public required long CategoryId { get; set; }

        [Association(ThisKey = "CategoryId", OtherKey = "Id")]
        public Category? Category { get; set; }
    }
}
