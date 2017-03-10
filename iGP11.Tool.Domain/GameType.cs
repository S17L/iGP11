using iGP11.Library.Attributes;
using iGP11.Library.DDD;

namespace iGP11.Tool.Domain
{
    [DefaultValue((int)DarkSouls3)]
    public enum GameType
    {
        [AggregateId("{6F3197DA-23A0-42E4-8B6E-2DD2AE86558D}")]
        [ResourceKey("DarkSouls2")]
        DarkSouls2,

        [AggregateId("{3B694522-C333-447A-B080-28F6DFDE9CD0}")]
        [ResourceKey("DarkSouls3")]
        DarkSouls3,

        [AggregateId("{26E40BAF-4574-44CA-99C3-508F45928D6C}")]
        [ResourceKey("Fallout4")]
        Fallout4
    }
}