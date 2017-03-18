using System.Collections.Generic;

using iGP11.Library.Component;

namespace iGP11.Tool.Shared.Plugin
{
    public class PluginComponent
    {
        public PluginComponent(
            IEnumerable<EffectType> supportedEffectTypes,
            IEnumerable<IComponent> elements,
            IEnumerable<IEffect> effects)
        {
            SupportedEffectTypes = supportedEffectTypes;
            Elements = elements;
            Effects = effects;
        }

        public IEnumerable<IEffect> Effects { get; }

        public IEnumerable<IComponent> Elements { get; }

        public IEnumerable<EffectType> SupportedEffectTypes { get; }
    }
}