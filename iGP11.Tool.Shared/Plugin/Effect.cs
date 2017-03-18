using System;

using iGP11.Library.Component;

namespace iGP11.Tool.Shared.Plugin
{
    public class Effect<TEffect> : IEffect
    {
        private readonly TEffect _data;
        private readonly IEffectSerializer _serializer;
        private readonly EffectType _type;

        public Effect(
            Guid id,
            EffectType type,
            bool isEnabled,
            TEffect data,
            ConcreteComponentAssembler assembler,
            IEffectSerializer serializer)
        {
            _type = type;
            _data = data;
            Component = assembler.Assemble(_data);
            _serializer = serializer;

            Id = id;
            IsEnabled = isEnabled;
        }

        public IComponent Component { get; }

        public Guid Id { get; }

        public bool IsEnabled { get; set; }

        public EffectData Serialize()
        {
            return new EffectData(
                Id,
                _type,
                IsEnabled,
                _serializer.Serialize(_data));
        }
    }
}