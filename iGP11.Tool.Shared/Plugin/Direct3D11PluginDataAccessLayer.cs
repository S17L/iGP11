using System.Collections.Generic;
using System.Linq;

using iGP11.Library.Component;
using iGP11.Tool.Shared.Model.GameSettings;

namespace iGP11.Tool.Shared.Plugin
{
    public class Direct3D11PluginDataAccessLayer : IPluginDataAccessLayer
    {
        private readonly ConcreteComponentAssembler _assembler;
        private readonly EffectFactory _effectFactory;
        private readonly Direct3D11Settings _settings;

        public Direct3D11PluginDataAccessLayer(
            Direct3D11Settings settings,
            ConcreteComponentAssembler assembler)
        {
            _settings = settings;
            _assembler = assembler;
            _effectFactory = new EffectFactory(_assembler);
        }

        public IEffect AddEffect(EffectType type)
        {
            var data = _effectFactory.Create(type);
            _settings.Effects.Add(data);

            return _effectFactory.Create(data);
        }

        public PluginComponent CreateComponent()
        {
            return new PluginComponent(
                GetSupportedEffectTypes().ToArray(),
                GetSettings().ToArray(),
                GetEffects().ToArray());
        }

        public void UpdateEffects(IEnumerable<EffectData> effects)
        {
            _settings.Effects.Clear();
            _settings.Effects.AddRange(effects);
        }

        private static IEnumerable<EffectType> GetSupportedEffectTypes()
        {
            yield return EffectType.BokehDoF;
            yield return EffectType.Denoise;
            yield return EffectType.LiftGammaGain;
            yield return EffectType.Lumasharpen;
            yield return EffectType.Tonemap;
            yield return EffectType.Vibrance;
        }

        private IEnumerable<IEffect> GetEffects()
        {
            return _settings.Effects.Select(_effectFactory.Create);
        }

        private IEnumerable<IComponent> GetSettings()
        {
            yield return _assembler.Assemble(_settings.PluginSettings);
            yield return _assembler.Assemble(_settings.Textures);
            yield return _assembler.Assemble(_settings.DepthBuffer);
        }
    }
}