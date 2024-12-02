using System.ComponentModel;

namespace _102techBot.Extensions
{
    internal static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            if (field != null)
            {
                var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
                return attribute?.Description ?? value.ToString();
            }

            return value.ToString();
        }
    }
}
