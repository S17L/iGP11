using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model
{
    public enum Srgb
    {
        [ComponentName("None")]
        [Editable]
        None = 0,

        [ComponentName("In")]
        [Editable]
        In,

        [ComponentName("Out")]
        [Editable]
        Out,
        Both
    }
}