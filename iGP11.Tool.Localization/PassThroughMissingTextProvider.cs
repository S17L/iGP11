namespace iGP11.Tool.Localization
{
    public class PassThroughMissingTextProvider : IMissingTextProvider
    {
        public string Get(string key)
        {
            return key;
        }
    }
}