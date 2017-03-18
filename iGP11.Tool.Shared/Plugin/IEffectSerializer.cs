namespace iGP11.Tool.Shared.Plugin
{
    public interface IEffectSerializer
    {
        string Serialize<TEffect>(TEffect effect);
    }
}