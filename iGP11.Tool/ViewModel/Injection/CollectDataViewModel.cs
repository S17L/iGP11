using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.ViewModel.Injection
{
    public class CollectDataViewModel : ViewModel,
                                        IInjectionViewModel
    {
        public string ApplicationFilePath { get; set; }

        public string FormattedConfigurationDirectoryPath => ProxyDirectoryPath;

        public string FormattedLogsDirectoryPath => LogsDirectoryPath;

        public string LogsDirectoryPath { get; set; }

        public PluginType PluginType { get; set; }

        public string ProxyDirectoryPath { get; set; }
    }
}