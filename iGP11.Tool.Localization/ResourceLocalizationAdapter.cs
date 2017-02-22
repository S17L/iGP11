namespace iGP11.Tool.Localization
{
    public class ResourceLocalizationAdapter : Localization
    {
        private readonly IMissingTextProvider _missingTextProvider;
        private readonly ITextProcessor _textProcessor;

        public ResourceLocalizationAdapter(IMissingTextProvider missingTextProvider = null, ITextProcessor textProcessor = null)
        {
            _missingTextProvider = missingTextProvider ?? new PassThroughMissingTextProvider();
            _textProcessor = textProcessor ?? new PassThroughTextProcessor();
        }

        public override string Get(string key)
        {
            return _textProcessor.Process(Resources.ResourceManager.GetString(key) ?? _missingTextProvider.Get(key));
        }
    }
}