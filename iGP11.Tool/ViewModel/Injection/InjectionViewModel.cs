using iGP11.Library.Component;
using iGP11.Tool.Application.Api.Model;
using iGP11.Tool.Shared.Model;
using iGP11.Tool.Shared.Model.GameSettings;

namespace iGP11.Tool.ViewModel.Injection
{
    public class InjectionViewModel : ViewModel,
                                      IInjectionViewModel
    {
        private readonly InjectionConfigurationViewModel _injectionConfigurationViewModel;
        private readonly GamePackage _package;
        private readonly ITokenReplacer _tokenReplacer;

        public InjectionViewModel(InjectionConfigurationViewModel injectionConfigurationViewModel, GamePackage package)
        {
            _injectionConfigurationViewModel = injectionConfigurationViewModel;
            _package = package;
            _tokenReplacer = new TokenReplacer(new ApplicationFilePathTokenReplacingPolicy(() => _package.Game?.FilePath ?? string.Empty));
        }

        public string FormattedConfigurationDirectoryPath => _package.GameProfile != null
                                                                 ? _tokenReplacer.Replace(_package.GameProfile.ProxyDirectoryPath)
                                                                 : string.Empty;

        public string FormattedLogsDirectoryPath => _package.GameProfile != null
                                                        ? _tokenReplacer.Replace(_package.GameProfile.LogsDirectoryPath)
                                                        : string.Empty;

        public string GameFilePath
        {
            get { return _package.Game?.FilePath ?? string.Empty; }
            set
            {
                if ((_package.Game == null) || (_package.Game.FilePath == value))
                {
                    return;
                }

                _package.Game.FilePath = value;
                _injectionConfigurationViewModel.PluginViewModel?.Rebind();
            }
        }

        public string LogsDirectoryPath
        {
            get { return _package.GameProfile?.LogsDirectoryPath ?? string.Empty; }
            set
            {
                if ((_package.GameProfile == null) || (_package.GameProfile.LogsDirectoryPath == value))
                {
                    return;
                }

                _package.GameProfile.LogsDirectoryPath = value;
            }
        }

        public PluginType PluginType
        {
            get { return _package.GameProfile?.PluginType ?? PluginType.Direct3D11; }
            set
            {
                if ((_package.GameProfile == null) || (_package.GameProfile.PluginType == value))
                {
                    return;
                }

                _package.GameProfile.PluginType = value;
            }
        }

        public string ProxyDirectoryPath
        {
            get { return _package.GameProfile?.ProxyDirectoryPath ?? string.Empty; }
            set
            {
                if ((_package.GameProfile == null) || (_package.GameProfile.ProxyDirectoryPath == value))
                {
                    return;
                }

                _package.GameProfile.ProxyDirectoryPath = value;
            }
        }

        public string GameName
        {
            get { return _package.Game?.Name; }
            set
            {
                if (_package.Game != null)
                {
                    _package.Game.Name = value;
                }
            }
        }

        public string GameProfileName
        {
            get { return _package.GameProfile?.Name; }
            set
            {
                if (_package.GameProfile != null)
                {
                    _package.GameProfile.Name = value;
                }
            }
        }
    }
}