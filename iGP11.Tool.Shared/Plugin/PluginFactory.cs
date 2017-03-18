using System;
using System.Collections.Generic;

using iGP11.Library.Component;
using iGP11.Library.Component.DataAnnotations;
using iGP11.Tool.Shared.Model;
using iGP11.Tool.Shared.Model.GameSettings;

namespace iGP11.Tool.Shared.Plugin
{
    public class PluginFactory : IPluginFactory
    {
        private readonly IDictionary<PluginType, Func<GamePackage, IPluginDataAccessLayer>> _gameProfileFactory;
        private readonly IDictionary<PluginType, Func<ProxyPluginSettings, IPluginDataAccessLayer>> _proxyPluginSettingsFactory;

        public PluginFactory(ComponentAssembler assembler)
        {
            _gameProfileFactory = new Dictionary<PluginType, Func<GamePackage, IPluginDataAccessLayer>>
            {
                [PluginType.Direct3D11] = package => new Direct3D11PluginDataAccessLayer(
                    package.GameProfile.Direct3D11Settings,
                    GetAssembler(package.Game, assembler))
            };

            _proxyPluginSettingsFactory = new Dictionary<PluginType, Func<ProxyPluginSettings, IPluginDataAccessLayer>>
            {
                [PluginType.Direct3D11] = settings => new Direct3D11PluginDataAccessLayer(
                    settings.Direct3D11Settings,
                    new ConcreteComponentAssembler(assembler, new AssemblingContext(FormType.Edit)))
            };
        }

        public IPluginDataAccessLayer Create(GamePackage package)
        {
            if (!_gameProfileFactory.ContainsKey(package.GameProfile.PluginType))
            {
                throw new ArgumentException($@"plugin component policy for {package.GameProfile.PluginType} could not be resolved", nameof(package));
            }

            return _gameProfileFactory[package.GameProfile.PluginType](package);
        }

        public IPluginDataAccessLayer Create(ProxyPluginSettings settings)
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