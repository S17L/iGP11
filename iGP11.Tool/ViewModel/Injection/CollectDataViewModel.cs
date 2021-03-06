using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.ViewModel.Injection
{
    public class CollectDataViewModel : ViewModel,
                                        IInjectionViewModel
    {
        public string FormattedConfigurationDirectoryPath => ProxyDirectoryPath;

        public string FormattedLogsDirectoryPath => LogsDirectoryPath;

        public string GameFilePath { get; set; }

        public string LogsDirectoryPath { get; set; }

        public PluginType PluginType { get; set; }

        public string ProxyDirectoryPath { get; set; }

        public string GameName { get; set; }

        public string GameProfileName { get; set; }
    }
}