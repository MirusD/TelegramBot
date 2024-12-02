using _102techBot.Domain.Entities;

namespace _102techBot.Domain
{
    internal class TemporaryRepair
    {
        public long UserId { get; set; }
        public User? User { get; set; }
        public string? Phone { get; set; }
        public long? ProductId { get; set; }
        public Product? Product { get; set; }
        public long? CategoryId { get; set; }
        public Category? Category { get; set; }
        public string? Description { get; set; }
        public RepairStatus Status { get; set; } = RepairStatus.Creating;
    }
}
