using _102techBot.Domain.Entities;
using LinqToDB.Mapping;

namespace _102techBot.Domain.Entities
{
    [Table(Name = "OrderItems")]
    internal class OrderItem
    {
        [PrimaryKey, Identity]
        [Column(Name = "Id")]
        public long Id { get; set; }

        [Column(Name = "OrderId")]
        public long OrderId { get; set; }

        [Association(ThisKey = "OrderId", OtherKey = "Id")]
        public Order Order { get; set; }

        [Column(Name = "ProductId")]
        public long ProductId { get; set; }

        [Association(ThisKey = "ProductId", OtherKey = "Id")]
        public Product Product { get; set; }

        [Column(Name = "Quantity")]
        public int Quantity { get; set; }

        [Column(Name = "Price")]
        public decimal Price { get; set; }
    }
}
