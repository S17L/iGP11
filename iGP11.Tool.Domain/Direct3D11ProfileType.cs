using iGP11.Library.Attributes;
using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Domain
{
    [DefaultValue((int)DarkSouls3)]
    public enum Direct3D11ProfileType
    {
        [Editable]
        Generic = 0,

        [Editable]
        DarkSouls2,

        [Editable]
        DarkSouls3,

        [Editable]
        Fallout4
    }
}