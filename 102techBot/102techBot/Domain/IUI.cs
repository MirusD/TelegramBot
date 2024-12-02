using _102techBot.Common;
using _102techBot.DTOs;
using Telegram.Bot.Types;
using static _102techBot.Infrastructure.UI.TelegramService;

namespace _102techBot.Domain
{
    internal interface IUI
    {
        public HandlerOnMessage? HandlersOnMessage { get; set; }
        public HandlerOnUpdate? HandlersOnUpdate { get; set; }
        public Task Init();
        public Task RemoveKeyboard(long chatId, int messageId);
        public Task<Message> SendMessageAsync(long chatID, string message);
        public Task<Message> SendMessageAsync(long chatID, string message, List<Button> keyboard);
        public Task<Message> SendMessageAsync(long chatID, int messageId, string message, List<Button> keyboard);
        public Task<Message> SendMessageAsync(long chatId, int messageId, string message);
        public Task EditMessageAsync(long chatID, int messageId, string message,  List<Button>? keyboard = null);
        public Task SendPushMessage(string? callbackQueryId, string text);
        public Task<Message> SendPhotoAsync(long chatId, int messageId, string filePath, string caption = "", List<Button>? keyboard = null);
        public Task<string?> GetPhoto(string fileId);
        public Task RequestPhoneNumberAsync(long chatId);
        public Task SendPaginatedMessage<T>(
            long chatId,
            int messageId,
            int page,
            List<T> data,
            Func<T, string> formatDataItem,
            Func<int, List<Button>> createNavigationButtons,
            int itemsPerPage = 1);
        public Task SendPhotoPaginatedMessage<T>(
            long chatId,
            int messageId,
            int page,
            List<T> data,
            Func<T, string> formatDataItem,
            Func<T, string> getFilePath,
            Func<T, List<Button>> createProductButtons,
            Func<int, List<Button>>? createNavigationButtons = null,
            int itemsPerPage = 3);
        public Task RemoveMessageAsync(long chatId, int messageId);
        public abstract Task DeleteMessagesInBatchesAsync(
            long chatId,
            int batchSize = 15,
            int delayMilliseconds = 1000);
        public Task RemoveBtnsAsync(long chatId, int messageId);
    }
}
