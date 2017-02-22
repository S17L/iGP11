using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.ViewModel.Injection
{
    public class CollectDataViewModel : ViewModel,
                                        IInjectionViewModel
    {
        public string ApplicationFilePath { get; set; }

        public string ConfigurationDirectoryPath { get; set; }

        public string FormattedConfigurationDirectoryPath => ConfigurationDirectoryPath;

        public string FormattedLogsDirectoryPath => LogsDirectoryPath;

        public string LogsDirectoryPath { get; set; }

        public PluginType PluginType { get; set; }
    }
}