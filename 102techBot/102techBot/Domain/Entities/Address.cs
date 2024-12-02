using LinqToDB.Mapping;

namespace _102techBot.Domain.Entities
{
    [Table(Name = "Addresses")]
    internal class Address
    {
        [PrimaryKey, Identity]
        [Column(Name = "Id")]
        public long Id { get; set; }
        [Column(Name = "Street")]
        public required string Street { get; set; }
        [Column(Name = "City")]
        public required string City { get; set; }
        [Column(Name = "PostalCode")]
        public required string PostalCode { get; set; }
        [Column(Name = "Country")]
        public required string Country { get; set; }
        [Column(Name = "Description")]
        public string? Description { get; set; }
        [Column(Name = "Latitude")]
        public required double Latitude { get; set; }
        [Column(Name = "Longitude")]
        public required double Longitude { get; set; }
        [Column(Name = "WorkingHours")]
        public required string WorkingHours { get; set; }
        [Column(Name = "Phone")]
        public required string Phone { get; set; }
    }
}