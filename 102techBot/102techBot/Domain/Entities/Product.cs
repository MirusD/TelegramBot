using LinqToDB.Mapping;

namespace _102techBot.Domain.Entities
{
    enum ProductStatus
    {
        Draft,
        Active
    }
    [Table(Name = "Products")]
    internal class Product
    {
        [PrimaryKey, Identity]
        [Column(Name = "Id")]
        public long Id { get; set; }

        [Column(Name = "Name")]
        public string? Name { get; set; }

        [Column(Name = "Description")]
        public string? Description { get; set; }

        [Column(Name = "Price")]
        public decimal Price { get; set; }

        [Column(Name = "Stock")]
        public int Stock { get; set; }

        [Column(Name = "ImageUrl")]
        public string? ImageUrl { get; set; }

        [Column(Name = "CategoryId")]
        public long? CategoryId { get; set; }

        [Association(ThisKey = "CategoryId", OtherKey = "Id")]
        public Category? Category { get; set; }

        [Column(Name = "UserId")]
        public long UserId { get; set; }

        [Association(ThisKey = "UserId", OtherKey = "Id")]
        public User? User { get; set; }

        [Column(Name = "Status")]
        public ProductStatus Status { get; set; }
    }
}
