using iGP11.Library.Component;
using iGP11.Tool.Application.Api.Model;
using iGP11.Tool.Shared.Model;
using iGP11.Tool.Shared.Model.InjectionSettings;

namespace iGP11.Tool.ViewModel.Injection
{
    public class InjectionViewModel : ViewModel,
                                      IInjectionViewModel
    {
        private readonly IInjectionConfigurationViewModel _injectionConfigurationViewModel;
        private readonly InjectionSettings _settings;
        private readonly ITokenReplacer _tokenReplacer;

        public InjectionViewModel(IInjectionConfigurationViewModel injectionConfigurationViewModel, InjectionSettings settings)
        {
            _injectionConfigurationViewModel = injectionConfigurationViewModel;
            _settings = settings;
            _tokenReplacer = new TokenReplacer(new ApplicationFilePathTokenReplacingPolicy(() => _settings?.ApplicationFilePath ?? string.Empty));
        }

        public string ApplicationFilePath
        {
            get { return _settings?.ApplicationFilePath ?? string.Empty; }
            set
            {
                if ((_settings == null) || (_settings.ApplicationFilePath == value))
                {
                    return;
                }

                _settings.ApplicationFilePath = value;
                _injectionConfigurationViewModel.RebindPlugin();
            }
        }

        public string FormattedConfigurationDirectoryPath => _settings != null
                                                                 ? _tokenReplacer.Replace(_settings.ProxyDirectoryPath)
                                                                 : string.Empty;

        public string FormattedLogsDirectoryPath => _settings != null
                                                        ? _tokenReplacer.Replace(_settings.LogsDirectoryPath)
                                                        : string.Empty;

        public string LogsDirectoryPath
        {
            get { return _settings?.LogsDirectoryPath ?? string.Empty; }
            set
            {
                if ((_settings == null) || (_settings.LogsDirectoryPath == value))
                {
                    return;
                }

                _settings.LogsDirectoryPath = value;
            }
        }

        public PluginType PluginType
        {
            get { return _settings?.PluginType ?? PluginType.Direct3D11; }
            set
            {
                if ((_settings == null) || (_settings.PluginType == value))
                {
                    return;
                }

                _settings.PluginType = value;
            }
        }

        public string ProxyDirectoryPath
        {
            get { return _settings?.ProxyDirectoryPath ?? string.Empty; }
            set
            {
                if ((_settings == null) || (_settings.ProxyDirectoryPath == value))
                {
                    return;
                }

                _settings.ProxyDirectoryPath = value;
            }
        }
    }
}