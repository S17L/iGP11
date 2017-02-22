using System;
using System.Collections.Generic;

using iGP11.Library.Component;
using iGP11.Library.Component.DataAnnotations;
using iGP11.Tool.Application.Api.Model;
using iGP11.Tool.Shared.Model;
using iGP11.Tool.Shared.Model.InjectionSettings;

namespace iGP11.Tool.Model
{
    public class PluginComponentFactory : IPluginComponentFactory
    {
        private readonly IDictionary<PluginType, Func<InjectionSettings, IComponent>> _injectionSettingsFactory;
        private readonly IDictionary<PluginType, Func<ProxyPluginSettings, IComponent>> _proxyPluginSettingsFactory;

        public PluginComponentFactory(ComponentAssembler assembler)
        {
            _injectionSettingsFactory = new Dictionary<PluginType, Func<InjectionSettings, IComponent>>
            {
                [PluginType.Direct3D11] = settings => GetAssembler(settings, assembler).Assemble(settings.Direct3D11Settings)
            };

            _proxyPluginSettingsFactory = new Dictionary<PluginType, Func<ProxyPluginSettings, IComponent>>
            {
                [PluginType.Direct3D11] = settings => assembler.Assemble(settings.Direct3D11Settings, new AssemblingContext(FormType.Edit))
            };
        }

        public IComponent Create(InjectionSettings settings)
        {
            if (!_injectionSettingsFactory.ContainsKey(settings.PluginType))
            {
                throw new ArgumentException($@"plugin component policy for {settings.PluginType} could not be resolved", nameof(settings));
            }

            return _injectionSettingsFactory[settings.PluginType](settings);
        }

        public IComponent Create(ProxyPluginSettings settings)
        {
            if (!_proxyPluginSettingsFactory.ContainsKey(settings.PluginType))
            {
                throw new ArgumentException($@"plugin component policy for {settings.PluginType} could not be resolved", nameof(settings));
            }

            return _proxyPluginSettingsFactory[settings.PluginType](settings);
        }

        private static ConcreteComponentAssembler GetAssembler(InjectionSettings settings, ComponentAssembler assembler)
        {
            var tokenReplacer = new TokenReplacer(new ApplicationFilePathTokenReplacingPolicy(() => settings.ApplicationFilePath));
            var assemblingContext = new AssemblingContext(FormType.New, tokenReplacer);

            return new ConcreteComponentAssembler(assembler, assemblingContext);
        }
    }
}