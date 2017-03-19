using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model.GameSettings
{
    [ComponentName("HDR")]
    [ComponentLongDescription("HDRLongDescription")]
    [DataContract]
    public class Hdr
    {
        private const string GeneralGroupBy = "General";

        [ComponentName("Strength")]
        [DataMember(Name = "strength")]
        [Editable]
        [FloatRange(0, 8)]
        [GroupedBy(GeneralGroupBy)]
        [Order(2)]
        public float Strength { get; set; } = 1.3f;

        [ComponentName("Radius")]
        [DataMember(Name = "radius")]
        [Editable]
        [FloatRange(0, 8)]
        [GroupedBy(GeneralGroupBy)]
        [Order(1)]
        public float Radius { get; set; } = 0.87f;
    }
}