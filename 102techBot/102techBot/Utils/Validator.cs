using System.Text.RegularExpressions;

namespace _102techBot.Utils
{
    internal static class Validator
    {
        public static bool IsValidRussianPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            string pattern = @"^\+7\d{10}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }
    }
}
