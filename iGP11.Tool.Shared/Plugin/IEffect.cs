using System;

using iGP11.Library.Component;

namespace iGP11.Tool.Shared.Plugin
{
    public interface IEffect
    {
        IComponent Component { get; }

        Guid Id { get; }

        bool IsEnabled { get; set; }

        EffectData Serialize();
    }
}