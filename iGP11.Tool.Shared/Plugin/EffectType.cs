using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Plugin
{
    public enum EffectType
    {
        [ComponentName("BokehDoF")]
        BokehDoF,

        [ComponentName("Denoise")]
        Denoise,

        [ComponentName("HDR")]
        Hdr,

        [ComponentName("LiftGammaGain")]
        LiftGammaGain,

        [ComponentName("LumaSharpen")]
        Lumasharpen,

        [ComponentName("Tonemap")]
        Tonemap,

        [ComponentName("Vibrance")]
        Vibrance
    }
}