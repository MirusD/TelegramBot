namespace _102techBot.Common
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Paginator
    {
    public static (List<Button> btns, string messageText, string filePath) PaginatedMessage<T>(
        long chatId,
        int page,
        List<T> data,
        Func<T, string> formatDataItem, // Форматирование текста для элемента
        Func<int, List<Button>> createNavigationButtons, // Создание кнопок навигации
        Func<T, string>? getFilePath = null, // Извлечение пути к файлу (опционально)
        int itemsPerPage = 1)
        {
            int totalPages = (int)Math.Ceiling(data.Count / (double)itemsPerPage);

            // Получаем данные для текущей страницы
            var pageData = data.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            // Формируем текст сообщения
            var messageText = $"Страница {page}/{totalPages}\n\n" +
                              string.Join("\n", pageData.Select(formatDataItem));

            // Извлекаем путь к файлу для первого элемента страницы (если делегат указан)
            string? filePath = (getFilePath != null && pageData.Count > 0)
                ? getFilePath(pageData[0])
                : "";

            // Создаём кнопки
            var btns = createNavigationButtons(page);

            if (page > 1)
            {
                btns.Add(new Button
                {
                    Text = "⬅️ Назад",
                    Data = $"/page/{page - 1}",
                });
            }

            if (page < (int)Math.Ceiling(data.Count / (double)1))
            {
                btns.Add(new Button
                {
                    Text = "Вперёд ➡️",
                    Data = $"/page/{page + 1}",
                });
            }

            return (btns, messageText, filePath);
        }
    }
}
