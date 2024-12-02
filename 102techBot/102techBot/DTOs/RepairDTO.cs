using _102techBot.Domain.Entities;

namespace _102techBot.DTOs
{
    internal class RepairDTO
    {
        public long Id { get; set; }
        public required string UserName { get; set; }
        public required string ProductName { get; set; }
        public required string? Description { get; set; }
        public required string Phone { get; set; }
        public required RepairStatus Status { get; set; }
        public required string CategoryName { get; set; }
    }
}
