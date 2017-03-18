using System.Collections.Generic;

namespace iGP11.Tool.Shared.Plugin
{
    public interface IPluginDataAccessLayer
    {
        IEffect AddEffect(EffectType type);

        PluginComponent CreateComponent();

        void UpdateEffects(IEnumerable<EffectData> effects);
    }
}