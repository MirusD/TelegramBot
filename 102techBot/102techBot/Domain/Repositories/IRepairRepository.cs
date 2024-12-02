using _102techBot.Domain.Entities;

namespace _102techBot.Domain.Repositories
{
    internal interface IRepairRepository
    {
        public Task<Repair> AddAsync(Repair newRepair);
        public Task<Repair?> DeleteAsync(long repairId);
        public Task<List<Repair>> GetAllAsync();
        public Task<Repair?> GetByIdAsync(long repairId);
        public Task<List<Repair>> GetRepairesByUserAsync(long userId);
        public Task<Repair?> UpdateAsync(Repair updatedRepair);
    }
}
