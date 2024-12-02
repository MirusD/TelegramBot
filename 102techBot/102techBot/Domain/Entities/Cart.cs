using LinqToDB.Mapping;

namespace _102techBot.Domain.Entities
{
    [Table(Name = "Carts")]
    internal class Cart
    {
        [PrimaryKey, Identity]
        [Column(Name = "Id")]
        public long Id { get; set; }

        [Column(Name = "UserId")]
        public long UserId { get; set; }

        [Association(ThisKey = "UserId", OtherKey = "Id")]
        public User User { get; set; }
    }
}
