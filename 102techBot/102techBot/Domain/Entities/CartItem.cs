using LinqToDB.Mapping;

namespace _102techBot.Domain.Entities
{
    [Table(Name = "CartItems")]
    internal class CartItem
    {
        [PrimaryKey, Identity]
        [Column(Name = "Id")]
        public long Id { get; set; }

        [Column(Name = "CartId")]
        public required long CartId { get; set; }

        [Association(ThisKey = "CartId", OtherKey = "Id")]
        public Cart? Cart { get; set; }

        [Column(Name = "ProductId")]
        public required long ProductId { get; set; }

        [Association(ThisKey = "ProductId", OtherKey = "Id")]
        public Product? Product { get; set; }

        [Column(Name = "Quantity")]
        public required int Quantity { get; set; }
    }
}
