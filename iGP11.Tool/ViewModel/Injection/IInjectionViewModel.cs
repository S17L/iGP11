using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.ViewModel.Injection
{
    public interface IInjectionViewModel
    {
        string FormattedConfigurationDirectoryPath { get; }

        string FormattedLogsDirectoryPath { get; }

        string GameFilePath { get; set; }

        string LogsDirectoryPath { get; set; }

        PluginType PluginType { get; set; }

        string ProxyDirectoryPath { get; set; }
    }
}