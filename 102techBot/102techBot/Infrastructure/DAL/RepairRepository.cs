using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using LinqToDB;
using LinqToDB.Async;
using System.Transactions;
using Telegram.Bot.Types;
using static _102techBot.Program;

namespace _102techBot.Infrastructure.DAL
{
    internal class RepairRepository : IRepairRepository
    {
        private readonly Dictionary<long, Repair> _repaires = new Dictionary<long, Repair>();

        private readonly AppDbContext _db;

        public RepairRepository(AppDbContext dbContext)
        {
            _db = dbContext;
        }
        public async Task<Repair> AddAsync(Repair newRepair)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var id = await _db.InsertWithInt64IdentityAsync(newRepair);
                    await transaction.CommitAsync();

                    newRepair.Id = id;
                    return newRepair;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при добавлении заявки на ремонт" + ex.ToString());
                    throw;
                }
            }
        }

        public async Task<Repair?> DeleteAsync(long repairId)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var existingRepair = await _db.GetTable<Repair>()
                        .FirstOrDefaultAsync(r => r.Id == repairId);

                    if (existingRepair == null)
                    {
                        return null;
                    }

                    await _db.GetTable<Repair>()
                        .DeleteAsync(r => r.Id == repairId);

                    await transaction.CommitAsync();
                    return existingRepair;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при удалении заявки на ремонт" + ex.ToString());
                    throw;
                }
            }
        }

        public async Task<List<Repair>> GetAllAsync()
        {
            try
            {
                return await _db.GetTable<Repair>()
                    .LoadWith(r => r.User)
                    .LoadWith(r => r.Product)
                    .LoadWith(r => r.Category)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении всех заявок на ремонт" + ex.ToString());
                throw;
            }
        }

        public async Task<Repair?> GetByIdAsync(long repairId)
        {
            try
            {
                return await _db.GetTable<Repair>()
                                    .FirstOrDefaultAsync(c => c.Id == repairId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении заявки на ремонт по ID: {repairId}" + ex.ToString());
                throw;
            }
        }

        public async Task<List<Repair>> GetRepairesByUserAsync(long userId)
        {
            try
            {
                return await _db.GetTable<Repair>()
                    .Where(r => r.UserId == userId)
                    .ToListAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Ошибка при получении заявки на ремонт по пользователю" + ex.ToString());
                throw;
            }
        }

        public async Task<Repair?> UpdateAsync(Repair updatedRepair)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var existingRepair = await _db.GetTable<Repair>()
                                                .FirstOrDefaultAsync(r => r.Id == updatedRepair.Id);

                    if (existingRepair == null)
                    {
                        return null;
                    }

                    await _db.UpdateAsync(updatedRepair);
                    await transaction.CommitAsync();
                    return updatedRepair;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при обновлении заявки на ремонт" + ex.ToString());
                    throw;
                }
            }
        }
    }
}
