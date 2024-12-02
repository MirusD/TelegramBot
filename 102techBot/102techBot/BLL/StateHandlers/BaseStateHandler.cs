using _102techBot.BLL.Interfaces;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;
using _102techBot.Infrastructure.DAL;
using AutoMapper;

namespace _102techBot.BLL.StateHandlers
{
    internal abstract class BaseStateHandler : IStateHandler
    {
        protected readonly IUI _UI;
        protected readonly ITemporaryStorageRepository _storageRepository;
        protected readonly IMapper _mapper;

        public BaseStateHandler(IUI ui, ITemporaryStorageRepository temporaryStorageRepository, IMapper mapper)
        {
            _UI = ui;
            _storageRepository = temporaryStorageRepository;
            _mapper = mapper;
        }
        public abstract Task HandleStateAsync(User user, MessageDTO message);
    }
}
