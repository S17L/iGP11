using System.Text.RegularExpressions;

namespace iGP11.Tool.Localization
{
    public class AutoFormattedMissingTextProvider : IMissingTextProvider
    {
        private const string BreakCharacter = " ";

        private readonly Regex _regex = new Regex(
            @"(?<=[A-Z])(?=[A-Z][a-z]) |
            (?<=[^A-Z])(?=[A-Z]) |
            (?<=[A-Za-z])(?=[^A-Za-z])",
            RegexOptions.IgnorePatternWhitespace);

        public string Get(string key)
        {
            return Regex.Replace(_regex.Replace(key ?? string.Empty, BreakCharacter), @"\s+", BreakCharacter).ToLower();
        }
    }
}