using LinqToDB.Mapping;

namespace _102techBot.Domain.Entities
{
    [Table(Name = "Categories")]
    internal class Category
    {
        [PrimaryKey, Identity]
        [Column(Name = "Id")]
        public long Id { get; set; }

        [Column(Name = "Name")]
        public string Name { get; set; }

        [Column(Name = "Description"), Nullable]
        public string? Description { get; set; }
    }
}
