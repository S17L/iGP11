using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model
{
    public enum Direct3D11ProfileType
    {
        [ComponentName("Generic")]
        [Editable]
        Generic = 0,

        [ComponentName("DarkSouls2ScholarOfTheFirstSin")]
        [Editable]
        DarkSouls2,

        [ComponentName("DarkSouls3")]
        [Editable]
        DarkSouls3,

        [ComponentName("Fallout4")]
        [Editable]
        Fallout4
    }
}