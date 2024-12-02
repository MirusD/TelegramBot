using LinqToDB.Mapping;
using System.ComponentModel;

namespace _102techBot.Domain.Entities
{
    public enum CallbackStatus
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

    [Table(Name = "Callbacks")]
    internal class Callback
    {
        [PrimaryKey, Identity]
        [Column(Name = "Id")]
        public long Id { get; set; }

        [Column(Name = "UserId")]
        public required long UserId { get; set; }

        [Association(ThisKey = "UserId", OtherKey = "Id")]
        public User? User { get; set; }

        [Column(Name = "Phone")]
        public required string Phone { get; set; }

        [Column(Name = "Status")]
        public required CallbackStatus Status { get; set; }

    }
}
