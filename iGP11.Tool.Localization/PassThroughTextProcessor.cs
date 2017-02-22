namespace iGP11.Tool.Localization
{
    public class PassThroughTextProcessor : ITextProcessor
    {
        public string Process(string text)
        {
            return text;
        }
    }
}