using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using LinqToDB;
using static _102techBot.Program;

namespace _102techBot.Infrastructure.DAL
{
    internal class CallbackRepository : ICallbackRepository
    {
        private readonly AppDbContext _db;

        public CallbackRepository(AppDbContext dbContext)
        {
            _db = dbContext;
        }
        public async Task<Callback> AddAsync(Callback newCallback)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var id = await _db.InsertWithInt64IdentityAsync(newCallback);
                    await transaction.CommitAsync();

                    newCallback.Id = id;
                    return newCallback;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при добавлении заявки на ремонт " + ex.ToString());
                    throw;
                }
            }
        }

        public Task<Callback?> DeleteAsync(long callbackId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Callback>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Callback?> GetByIdAsync(long callbackId)
        {
            throw new NotImplementedException();
        }

        public Task<Callback> UpdateAsync(Callback updatedCallback)
        {
            throw new NotImplementedException();
        }
    }
}
