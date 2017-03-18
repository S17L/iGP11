using System;
using System.Runtime.Serialization;

namespace iGP11.Tool.Shared.Plugin
{
    [DataContract]
    public class EffectData
    {
        public EffectData(
            Guid id,
            EffectType type,
            bool isEnabled,
            string data)
        {
            Id = id;
            Type = type;
            IsEnabled = isEnabled;
            Data = data;
        }

        [DataMember(Name = "data", IsRequired = true)]
        public string Data { get; set; }

        [DataMember(Name = "id", IsRequired = true)]
        public Guid Id { get; private set; }

        [DataMember(Name = "isEnabled", IsRequired = true)]
        public bool IsEnabled { get; set; }

        [DataMember(Name = "type", IsRequired = true)]
        public EffectType Type { get; private set; }
    }
}