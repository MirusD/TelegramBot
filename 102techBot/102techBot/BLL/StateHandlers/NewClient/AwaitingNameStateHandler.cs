using _102techBot.Common;
using _102techBot.Domain;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using _102techBot.DTOs;
using AutoMapper;

namespace _102techBot.BLL.StateHandlers.NewClient
{
    /// <summary>
    /// Обработка состояния ожидания ввода имени пользователя при знакомстве с ботом
    /// </summary>
    internal class AwaitingNameStateHandler : BaseStateHandler
    {
        public AwaitingNameStateHandler(
            IUI ui, 
            ITemporaryStorageRepository temporaryStorageRepository, 
            IMapper mapper) 
            : base(ui, temporaryStorageRepository, mapper) {}

        public override async Task HandleStateAsync(User user, MessageDTO message)
        {
            _storageRepository.Add(user.Id.ToString(), message.Text);

            await _UI.SendMessageAsync(
            user.Id,
            $"Мы можем к вам обращаться как {message.Text}?",
            Buttons.GetConfirmBtns("confirm_name"));
        }
    }
}
