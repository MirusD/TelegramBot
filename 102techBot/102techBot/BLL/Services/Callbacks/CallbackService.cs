using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;

namespace _102techBot.BLL.Services.Callbacks
{
    internal class CallbackService
    {
        private readonly ICallbackRepository _callbackRepository;

        public CallbackService(ICallbackRepository callbackRepository)
        {
            _callbackRepository = callbackRepository;
        }

        public async Task<Callback> AddCallbackAsync(Callback newCallback)
        {
            return await _callbackRepository.AddAsync(newCallback);
        }
    }
}
