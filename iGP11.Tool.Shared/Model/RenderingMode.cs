using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model
{
    public enum RenderingMode
    {
        [ComponentName("RenderingModeAlpha")]
        [Editable]
        Alpha = 0,

        [ComponentName("RenderingModeDepthBuffer")]
        [Editable]
        DepthBuffer,

        [ComponentName("RenderingModeEffects")]
        [Editable]
        Effects,

        [ComponentName("RenderingModeLuminescence")]
        [Editable]
        Luminescence,

        [ComponentName("RenderingModeNone")]
        [Editable]
        None
    }
}