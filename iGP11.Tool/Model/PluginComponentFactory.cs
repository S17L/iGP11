using System;
using System.Collections.Generic;

using iGP11.Library.Component;
using iGP11.Library.Component.DataAnnotations;
using iGP11.Tool.Application.Api.Model;
using iGP11.Tool.ReadModel.Api.Model;
using iGP11.Tool.Shared.Model;
using iGP11.Tool.Shared.Model.GameSettings;

namespace iGP11.Tool.Model
{
    public class PluginComponentFactory : IPluginComponentFactory
    {
        private readonly IDictionary<PluginType, Func<GamePackage, IComponent>> _gameProfileFactory;
        private readonly IDictionary<PluginType, Func<ProxyPluginSettings, IComponent>> _proxyPluginSettingsFactory;

        public PluginComponentFactory(ComponentAssembler assembler)
        {
            _gameProfileFactory = new Dictionary<PluginType, Func<GamePackage, IComponent>>
            {
                [PluginType.Direct3D11] = package => GetAssembler(package.Game, assembler).Assemble(package.GameProfile.Direct3D11Settings)
            };

            _proxyPluginSettingsFactory = new Dictionary<PluginType, Func<ProxyPluginSettings, IComponent>>
            {
                [PluginType.Direct3D11] = settings => assembler.Assemble(settings.Direct3D11Settings, new AssemblingContext(FormType.Edit))
            };
        }

        public IComponent Create(GamePackage package)
        {
            if (!_gameProfileFactory.ContainsKey(package.GameProfile.PluginType))
            {
                throw new ArgumentException($@"plugin component policy for {package.GameProfile.PluginType} could not be resolved", nameof(package));
            }

            return _gameProfileFactory[package.GameProfile.PluginType](package);
        }

        public IComponent Create(ProxyPluginSettings settings)
        {
            if (!_proxyPluginSettingsFactory.ContainsKey(settings.PluginType))
            {
                throw new ArgumentException($@"plugin component policy for {settings.PluginType} could not be resolved", nameof(settings));
            }

            return _proxyPluginSettingsFactory[settings.PluginType](settings);
        }

        private static ConcreteComponentAssembler GetAssembler(Game game, ComponentAssembler assembler)
        {
            var tokenReplacer = new TokenReplacer(new ApplicationFilePathTokenReplacingPolicy(() => game.FilePath));
            var assemblingContext = new AssemblingContext(FormType.New, tokenReplacer);

            return new ConcreteComponentAssembler(assembler, assemblingContext);
        }
    }
}