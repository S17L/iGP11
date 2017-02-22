using System;
using System.Runtime.Serialization;

namespace iGP11.Library
{
    [DataContract]
    [Serializable]
    public class Localizable : IComparable<Localizable>
    {
        public Localizable(string key, params Localizable[] arguments)
            : this(key, true, arguments)
        {
        }

        public Localizable(string key, bool localizeEnabled, params Localizable[] arguments)
        {
            Key = key;
            LocalizeEnabled = localizeEnabled;
            Arguments = arguments;
        }

        [DataMember(Name = "arguments")]
        public Localizable[] Arguments { get; private set; }

        [DataMember(Name = "key")]
        public string Key { get; private set; }

        [DataMember(Name = "localizeEnabled")]
        public bool LocalizeEnabled { get; private set; }

        public static Localizable Empty()
        {
            return new Localizable(string.Empty, false);
        }

        public static Localizable NotLocalizable<TObject>(TObject @object)
        {
            return new Localizable(@object.ToString(), false);
        }

        public int CompareTo(Localizable other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            return ReferenceEquals(null, other)
                       ? 1
                       : string.Compare(Key, other.Key, StringComparison.Ordinal);
        }

        public override string ToString()
        {
            return Key;
        }
    }
}