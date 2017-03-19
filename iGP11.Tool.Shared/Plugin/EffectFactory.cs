using System;
using System.Collections.Generic;

using iGP11.Library;
using iGP11.Library.Component;
using iGP11.Tool.Shared.Model.GameSettings;

namespace iGP11.Tool.Shared.Plugin
{
    public class EffectFactory : IEffectSerializer
    {
        private readonly ConcreteComponentAssembler _assembler;
        private readonly IDictionary<EffectType, Func<EffectData, IEffect>> _effectFactories;
        private readonly IDictionary<EffectType, Func<EffectData>> _templateFactories;

        public EffectFactory(ConcreteComponentAssembler assembler)
        {
            _assembler = assembler;
            _effectFactories = new Dictionary<EffectType, Func<EffectData, IEffect>>
            {
                [EffectType.BokehDoF] = data => CreateEffect<BokehDoF>(data),
                [EffectType.Denoise] = data => CreateEffect<Denoise>(data),
                [EffectType.Hdr] = data => CreateEffect<Hdr>(data),
                [EffectType.LiftGammaGain] = data => CreateEffect<LiftGammaGain>(data),
                [EffectType.Lumasharpen] = data => CreateEffect<LumaSharpen>(data),
                [EffectType.Tonemap] = data => CreateEffect<Tonemap>(data),
                [EffectType.Vibrance] = data => CreateEffect<Vibrance>(data)
            };

            _templateFactories = new Dictionary<EffectType, Func<EffectData>>
            {
                [EffectType.BokehDoF] = () => CreateEffectData(EffectType.BokehDoF, new BokehDoF()),
                [EffectType.Denoise] = () => CreateEffectData(EffectType.Denoise, new Denoise()),
                [EffectType.Hdr] = () => CreateEffectData(EffectType.Hdr, new Hdr()),
                [EffectType.LiftGammaGain] = () => CreateEffectData(EffectType.LiftGammaGain, new LiftGammaGain()),
                [EffectType.Lumasharpen] = () => CreateEffectData(EffectType.Lumasharpen, new LumaSharpen()),
                [EffectType.Tonemap] = () => CreateEffectData(EffectType.Tonemap, new Tonemap()),
                [EffectType.Vibrance] = () => CreateEffectData(EffectType.Vibrance, new Vibrance())
            };
        }

        public IEffect Create(EffectData effect)
        {
            return _effectFactories[effect.Type](effect);
        }

        public EffectData Create(EffectType type)
        {
            return _templateFactories[type]();
        }

        string IEffectSerializer.Serialize<TEffect>(TEffect effect)
        {
            return effect.Serialize();
        }

        private static EffectData CreateEffectData<TData>(EffectType type, TData data)
        {
            return new EffectData(
                Guid.NewGuid(),
                type,
                true,
                data.Serialize());
        }

        private Effect<TData> CreateEffect<TData>(EffectData data)
        {
            return new Effect<TData>(
                data.Id,
                data.Type,
                data.IsEnabled,
                data.Data.Deserialize<TData>(),
                _assembler,
                this);
        }
    }
}