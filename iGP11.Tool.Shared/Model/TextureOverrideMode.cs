using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model
{
    public enum TextureOverrideMode
    {
        [ComponentName("None")]
        [Editable]
        None = 0,

        [ComponentName("Dumping")]
        [Editable]
        Dumping,

        [ComponentName("Override")]
        [Editable]
        Override
    }
}