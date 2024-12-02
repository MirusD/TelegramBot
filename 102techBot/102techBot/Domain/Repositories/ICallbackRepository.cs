using _102techBot.Domain.Entities;

namespace _102techBot.Domain.Repositories
{
    internal interface ICallbackRepository
    {
        public Task<Callback> AddAsync(Callback newCallback);
        public Task<Callback?> DeleteAsync(long callbackId);
        public Task<List<Callback>> GetAllAsync();
        public Task<Callback?> GetByIdAsync(long callbackId);
        public Task<Callback> UpdateAsync(Callback updatedCallback);
    }
}
