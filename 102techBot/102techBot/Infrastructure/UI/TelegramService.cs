using _102techBot.Common;
using _102techBot.Domain;
using _102techBot.DTOs;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using User = _102techBot.Domain.Entities.User;
using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;

namespace _102techBot.Infrastructure.UI
{
    internal class TelegramService : IUI
    {
        private readonly TelegramBotClient _botClient;
        private readonly CancellationTokenSource _cts;
        private readonly Dictionary<long, List<int>> _userSentMessages = new();
        private readonly IUserRepository _userRepository;

        public delegate Task HandlerOnMessage(User user, MessageDTO message);
        public delegate Task HandlerOnUpdate(User user, MessageDTO message);
        public HandlerOnMessage? HandlersOnMessage { get; set; }
        public HandlerOnUpdate? HandlersOnUpdate { get; set; }

        public TelegramService(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            var config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();
            var token = config["BotToken"];
            var cts = new CancellationTokenSource();
            _cts = cts;
            _botClient = new TelegramBotClient(token, cancellationToken: cts.Token);
            Init().Wait();
        }

        public async Task Init()
        {
            var botCommands = new List<BotCommand>();
            foreach (var command in Commands.GetMainMenuCommandsIntroducedUser())
            {
                botCommands.Add(new BotCommand()
                {
                    Command = command.Cmd,
                    Description = command.Description
                });
            }
            await _botClient.SetMyCommandsAsync(botCommands, cancellationToken: _cts.Token);
            var me = await _botClient.GetMeAsync();
            _botClient.OnMessage += OnMessage;
            _botClient.OnUpdate += OnUpdate;
            _botClient.OnError += OnError;
            Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
        }

        public async Task RemoveKeyboard(long chatId, int messageId)
        {
            await _botClient.EditMessageReplyMarkupAsync(
            chatId: chatId,
            messageId: messageId,
            replyMarkup: null
);
        }

        public async Task<Message> SendMessageAsync(long chatId, string message)
        {
            try
            {
                return await _botClient.SendTextMessageAsync(chatId, message);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Ошибка при отправке сообщения" + ex.ToString());
                throw;
            }
        }

        public async Task<Message> SendMessageAsync(long chatId, int messageId, string message)
        {
            await _botClient.EditMessageReplyMarkupAsync(
                chatId: chatId,
                messageId: messageId,
                replyMarkup: null
            );
            return await _botClient.SendTextMessageAsync(chatId, message);
        }

        public async Task<Message> SendMessageAsync(long chatId, string message, List<Button> keyboard)
        {
            var inlineKeyboard = new InlineKeyboardMarkup();

            foreach (var btn in keyboard)
            {
                if (!string.IsNullOrWhiteSpace(btn.Link))
                {
                    inlineKeyboard.AddButton(InlineKeyboardButton.WithUrl(btn.Text, btn.Link));
                }
                else
                {
                    inlineKeyboard.AddButton(btn.Text, btn.Data);
                }
                if (btn.Ln) inlineKeyboard.AddNewRow();
            }

            try
            {
                return await _botClient.SendTextMessageAsync(chatId, message, replyMarkup: inlineKeyboard);
            }
            catch(Exception ex) 
            {
                Console.WriteLine("Ошибка при отправке сообщения" + ex.ToString());
                throw;
            }
        }

        public async Task<Message?> SendMessageAsync(long chatId, int messageId, string message, List<Button> keyboard)
        {
            var inlineKeyboard = new InlineKeyboardMarkup();

            foreach (var btn in keyboard)
            {
                if (!string.IsNullOrWhiteSpace(btn.Link))
                {
                    inlineKeyboard.AddButton(InlineKeyboardButton.WithUrl(btn.Text, btn.Link));
                }
                else
                {
                    inlineKeyboard.AddButton(btn.Text, btn.Data);
                }
                if (btn.Ln) inlineKeyboard.AddNewRow();
            }
            try
            {
                return await _botClient.SendTextMessageAsync(chatId, message, replyMarkup: inlineKeyboard);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Ошибка при отправке сообщения" + ex.ToString());
                throw;
            }
        }

        public async Task EditMessageAsync(long chatId, int messageId, string messageText, List<Button>? keyboard = null)
        {
            if (keyboard != null)
            {
                var inlineKeyboard = new InlineKeyboardMarkup();

                foreach (var btn in keyboard)
                {
                    if (!string.IsNullOrWhiteSpace(btn.Link))
                    {
                        inlineKeyboard.AddButton(InlineKeyboardButton.WithUrl(btn.Text, btn.Link));
                    }
                    else
                    {
                        inlineKeyboard.AddButton(btn.Text, btn.Data);
                    }
                    if (btn.Ln) inlineKeyboard.AddNewRow();
                }

                try
                {
                    await _botClient.EditMessageTextAsync(chatId, messageId, text: messageText, replyMarkup: inlineKeyboard);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Ошибка при редактировании сообщения" + ex.ToString());
                    throw;
                }
            }
            else 
            {
                try
                {
                    await _botClient.EditMessageTextAsync(chatId, messageId, text: messageText, replyMarkup: null);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Ошибка при редактировании сообщения" + ex.ToString());
                    throw;
                }

            }
        }

        public async Task SendPushMessage(string? callbackQueryId, string text)
        {
            try
            {
                await _botClient.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQueryId,
                text: text,
                showAlert: true);
            }
            catch (Exception ex) 
            { 
                Console.WriteLine("Ошибка при отправке всплывающего сообщения" + ex.Message);
                throw;
            }
        }

        public async Task<Message> SendPhotoAsync(long chatId, int messageId, string filePath, string caption = "", List<Button>? keyboard = null)
        {
            var inlineKeyboard = new InlineKeyboardMarkup();

            if (keyboard != null) {
                foreach (var btn in keyboard)
                {
                    if (!string.IsNullOrWhiteSpace(btn.Link))
                    {
                        inlineKeyboard.AddButton(InlineKeyboardButton.WithUrl(btn.Text, btn.Link));
                    }
                    else
                    {
                        inlineKeyboard.AddButton(btn.Text, btn.Data);
                    }
                    if (btn.Ln) inlineKeyboard.AddNewRow();
                }
            }

            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var res = await _botClient.SendPhotoAsync(
                    chatId: chatId,
                    photo: stream,
                    caption: caption,
                    replyMarkup: inlineKeyboard
                );
                return res;
            }
            catch (Exception ex) 
            { 
                Console.WriteLine("Ошибка при отправке фотографии" + ex.Message.ToString());
                throw;
            }
        }

        public async Task<string?> GetPhoto(string fileId)
        {
            try
            {
                var file = await _botClient.GetFileAsync(fileId);

                if (file != null)
                {
                    string fileName = $"{Path.GetRandomFileName().Replace(".", "")}.jpg";

                    string imgFolder = Path.Combine(Directory.GetCurrentDirectory(), "img");
                    if (!Directory.Exists(imgFolder))
                    {
                        Directory.CreateDirectory(imgFolder);
                    }

                    string filePath = Path.Combine(imgFolder, fileName);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await _botClient.DownloadFileAsync(file.FilePath, memoryStream);

                        memoryStream.Position = 0;

                        using (var image = await Image.LoadAsync(memoryStream))
                        {
                            const int maxWidth = 300;
                            int newHeight = (int)(image.Height * ((float)maxWidth / image.Width));

                            image.Mutate(x => x.Resize(new ResizeOptions
                            {
                                Size = new Size(maxWidth, newHeight),
                                Mode = ResizeMode.Max
                            }));

                            await image.SaveAsync(filePath, new JpegEncoder());
                        }
                    }

                    return fileName;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке фото: {ex}");
                throw;
            }
        }

        public async Task RequestPhoneNumberAsync(long chatId)
        {
            var replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Поделиться номером телефона") { RequestContact = true }
                }
})
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };

            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Пожалуйста, поделитесь вашим номером телефона.",
                replyMarkup: replyKeyboardMarkup
            );
        }

        public async Task SendPaginatedMessage<T>(
            long chatId,
            int messageId,
            int page,
            List<T> data,
            Func<T, string> formatDataItem,
            Func<int, List<Button>> createNavigationButtons,
            int itemsPerPage = 1)
        {
            int totalPages = (int)Math.Ceiling(data.Count / (double)itemsPerPage);

            var pageData = data.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            var messageText = $"Страница {page}/{totalPages}\n\n" +
                              string.Join("\n", pageData.Select(formatDataItem));

            var btns = createNavigationButtons(page);

            await EditMessageAsync(chatId, messageId, messageText, btns);
        }

        public async Task SendPhotoPaginatedMessage<T>(
            long chatId,
            int messageId,
            int page,
            List<T> data,
            Func<T, string> formatDataItem,
            Func<T, string> getFilePath,
            Func<T, List<Button>>  createProductButtons,
            Func<int, List<Button>>? createNavigationButtons = null,
            int itemsPerPage = 3)
        {
            int totalPages = (int)Math.Ceiling(data.Count / (double)itemsPerPage);

            var pageData = data.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            List<Button> btns = createNavigationButtons != null ? createNavigationButtons(page) : new List<Button>();

            if (!_userSentMessages.ContainsKey(chatId))
            {
                _userSentMessages[chatId] = new List<int>();
            }

            foreach (var dataItem in pageData)
            {
                if (dataItem != null)
                {
                    try
                    {
                        var sentMessage = await SendPhotoAsync(
                            chatId,
                            messageId,
                            getFilePath(dataItem),
                            formatDataItem(dataItem),
                            createProductButtons(dataItem));

                        if (sentMessage != null)
                        {
                            _userSentMessages[chatId].Add(sentMessage.MessageId);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при отправке фотографии в чат: {chatId} " + ex.ToString());
                    }
                }
            }
            await SendMessageAsync(chatId, $"Страница {page}/{totalPages}", btns);
        }

        public async Task RemoveMessageAsync(long chatId, int messageId)
        {
            await _botClient.DeleteMessageAsync(chatId, messageId);
        }

        public async Task DeleteMessagesInBatchesAsync(
            long chatId,
            int batchSize = 15,
            int delayMilliseconds = 1000)
        {
            if (!_userSentMessages.ContainsKey(chatId)) { return; }
            for (int i = 0; i < _userSentMessages[chatId].Count; i += batchSize)
            {
                var batch = _userSentMessages[chatId].Skip(i).Take(batchSize);

                foreach (var messageId in batch)
                {
                    try
                    {
                        await _botClient.DeleteMessageAsync(chatId, messageId);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при удалении сообщения с ID {messageId}: {ex.Message}");
                    }
                }

                if (i + batchSize < _userSentMessages.Count)
                {
                    await Task.Delay(delayMilliseconds);
                }
            }

            _userSentMessages.Remove(chatId);
        }

        public async Task RemoveBtnsAsync(long chatId, int messageId)
        {
            try
            {
                await _botClient.EditMessageReplyMarkupAsync(
                    chatId: chatId,
                    messageId: messageId,
                    replyMarkup: null
                );
            }
            catch (Exception ex) { Console.WriteLine(ex); };
        }

        async Task OnError(Exception exception, HandleErrorSource source)
        {
            Console.WriteLine(exception);
        }

        async Task OnMessage(Message msg, UpdateType type)
        {
            var message = new MessageDTO 
            { 
                Id = msg.MessageId,
                Text = msg.Text,
                CallbackQueryId = null,
                PhotoFileId = msg.Photo != null ? msg.Photo[^1].FileId : null,
                IsPhoto = msg.Photo != null

            };

            var user = new User
            {
                Id = msg.Chat.Id,
                UserName = msg.Chat.Username,
                FirstName = msg.Chat.FirstName,
                LastName = msg.Chat.LastName,
                Role = UserRole.NewClient
            };
;
            HandlersOnMessage?.Invoke(user, message);
        }

        async Task OnUpdate(Update update)
        {
            if (update is { CallbackQuery: { } query })
            {
                var message = new MessageDTO
                {
                    Id = query.Message.MessageId,
                    Text = query.Data,
                    CallbackQueryId = query.Id,
                    PhotoFileId = null,
                    IsPhoto = query.Message.Photo != null,
                };

                var user = new User
                {
                    Id = query.From.Id,
                    UserName = query.From.Username,
                    FirstName = query.From.FirstName,
                    LastName = query.From.LastName,
                    Role = UserRole.NewClient
                };

                HandlersOnUpdate?.Invoke(user, message);
            }
        }
    }
}
