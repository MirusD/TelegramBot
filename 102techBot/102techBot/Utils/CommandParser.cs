using _102techBot.Common;
using System.Text.RegularExpressions;

namespace _102techBot.Utils
{
    internal static class CommandParser
    {
        public static (string cmd, string Option1, string Option2) Parse(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return (string.Empty, string.Empty, string.Empty);
            }

            var matches = Regex.Matches(command, @"/[^/]+");

            var result = matches.Count > 0
                ? matches.Cast<Match>().Select(m => m.Value)
                : new[] { command };

            string cmd = result.FirstOrDefault() ?? string.Empty;
            string opt1 = result.ElementAtOrDefault(1)?.TrimStart('/') ?? string.Empty;
            string opt2 = result.ElementAtOrDefault(2)?.TrimStart('/') ?? string.Empty;

            return (cmd, opt1, opt2);
        }
    }
}
