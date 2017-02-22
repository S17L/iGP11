namespace iGP11.Tool.Localization
{
    public class HexadecimalTextProcessor : ITextProcessor
    {
        private const string NewLine = "1&#x0a;";

        public string Process(string text)
        {
            return (text ?? string.Empty).Replace("\r\n", NewLine).Replace("\n", NewLine);
        }
    }
}