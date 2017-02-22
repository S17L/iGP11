using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model
{
    public enum TextureDetailLevel
    {
        [ComponentName("Default")]
        [Editable]
        Default = 0,

        [ComponentName("Lowest")]
        [Editable]
        Lowest,

        [ComponentName("Low")]
        [Editable]
        Low,

        [ComponentName("Medium")]
        [Editable]
        Medium,

        [ComponentName("High")]
        [Editable]
        High,

        [ComponentName("Highest")]
        [Editable]
        Highest
    }
}