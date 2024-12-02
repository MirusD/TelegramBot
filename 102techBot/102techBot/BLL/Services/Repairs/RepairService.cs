using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using Telegram.Bot.Types;

namespace _102techBot.BLL.Services.Repairs
{
    internal class RepairService
    {
        private readonly IRepairRepository _repairRepository;
        public RepairService(IRepairRepository repairRepository) 
        {
            _repairRepository = repairRepository;
        }

        public async Task<Repair> AddRepairAsync(Repair newRepair)
        {
            return await _repairRepository.AddAsync(newRepair);
        }

        public async Task<List<Repair>> GetAllUserRepairsAsync(long userId)
        {
            return await _repairRepository.GetRepairesByUserAsync(userId);
        }

        public async Task<Repair?> ChangeStatusAsync(long repairId, RepairStatus status)
        {
            var repair = await _repairRepository.GetByIdAsync(repairId);
            repair!.Status = status;

            var updatedRepair = await _repairRepository.UpdateAsync(repair);
            if (updatedRepair != null)
            {
                return updatedRepair;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<Repair>> GetAllAsync()
        {
            return await _repairRepository.GetAllAsync();
        }

        public async Task<Repair?> RemoveAsync(long id)
        {
            return await _repairRepository.DeleteAsync(id);
        }
    }
}
