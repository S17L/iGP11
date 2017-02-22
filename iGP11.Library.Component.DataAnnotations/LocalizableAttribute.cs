using System;

namespace iGP11.Library.Component.DataAnnotations
{
    public class LocalizableAttribute : Attribute
    {
        private readonly string _key;
        private readonly bool _localizeEnabled;

        protected LocalizableAttribute(string key, bool localizeEnabled = true)
        {
            _key = key;
            _localizeEnabled = localizeEnabled;
        }

        public Localizable Value => new Localizable(_key, _localizeEnabled);
    }
}