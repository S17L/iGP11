namespace iGP11.Tool.Localization
{
    public class Localization
    {
        private static Localization _current;

        public static Localization Current
        {
            get { return _current ?? (_current = new Localization()); }
            set { _current = value; }
        }

        public virtual string Get(string key)
        {
            return key;
        }
    }
}