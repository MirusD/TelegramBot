using System.Text;
using _102techBot.Extensions;
using _102techBot.Domain.Entities;

namespace _102techBot.Utils
{
    internal static class EntityFormatter
    {
        public static (long id, string userInfo) FormatUserInfo(User user)
        {
            var sb = new StringBuilder();

            if (Enum.IsDefined(typeof(UserRole), user.Role))
            {
                UserRole role = (UserRole)user.Role;
                sb.AppendLine($"Роль: {user.Role}");
            }
            else
            {
                sb.AppendLine($"Роль: Не определена");
            }
            sb.AppendLine($"Имя: {user.FirstName}");
            sb.AppendLine($"Фамилия: {user.LastName}");
            sb.AppendLine($"Ник: {user.UserName}");

            return (user.Id, sb.ToString());
        }
        public static Dictionary<long, String> FormatUserInfo(List<User> users)
        {
            var userCardList = new Dictionary<long, String>();
            foreach (var user in users)
            {
                var userInfo = FormatUserInfo(user);
                userCardList.Add(userInfo.Item1, userInfo.Item2);
            }
            return userCardList;
        }

        public static string FormatProductInfo(Product? product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var sb = new StringBuilder();
            sb.AppendLine($"Название: {product.Name}");
            sb.AppendLine($"Описание: {product.Description ?? "Нет описания"}");
            sb.AppendLine($"На складе: {product.Stock}");
            sb.AppendLine($"Цена: {product.Price:C}");
            return sb.ToString();
        }

        public static string FormatAddressInfo(Address address)
        {
            return
                $"Город: {address.City}\n" +
                $"ул.: {address.Street}\n" +
                $"Режим работы:\n{address.WorkingHours}\n" +
                $"Тел.: {address.Phone}\n";
        }

        public static string FormaRepairInfo(Repair repair)
        {
            return
                $"Тип: Заявка на ремонт\n" +
                $"Тип оборудования: {repair?.Category?.Name}\n" +
                $"Модель: {repair?.Product?.Name}\n" +
                $"Описание проблемы: {repair.Description}\n" +
                $"Тел.: {repair.Phone}\n" +
                $"Стату: {repair.Status.GetDescription()}";
        }

        public static string FormaOrderInfo(Order order)
        {
            return
                $"Тип: Заявка на покупку\n" +
                $"ИД заявки: {order.Id}\n" +
                $"Кто заказал: {order.User.FirstName}\n" +
                $"Количество: {order.TotalAmount}\n" +
                $"Тел.: {order.User.Phone}\n" +
                $"Стату: {order.Status.GetDescription()}";

        }

        public static string FormatCallbackInfo(string callback)
        {
            return
                $"Тип: Заявка на обратный звонок\n" +
                $"Номер телефона: {callback}";
        }
    }
}
