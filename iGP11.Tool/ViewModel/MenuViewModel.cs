using iGP11.Tool.Common;
using iGP11.Tool.Model;

namespace iGP11.Tool.ViewModel
{
    public sealed class MenuViewModel : ViewModel
    {
        private readonly INavigationService _navigationService;
        private IActionCommand _editConfigurationCommand;

        private IActionCommand _launchTextureManagementCommand;
        private IActionCommand _openAboutCommand;

        public MenuViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public IActionCommand EditConfigurationCommand
        {
            get { return _editConfigurationCommand ?? (_editConfigurationCommand = new ActionCommand(EditConfiguration, () => true)); }
        }

        public IActionCommand LaunchTextureManagementCommand
        {
            get { return _launchTextureManagementCommand ?? (_launchTextureManagementCommand = new ActionCommand(LaunchTextureManagement, () => true)); }
        }

        public IActionCommand OpenAboutCommand
        {
            get { return _openAboutCommand ?? (_openAboutCommand = new ActionCommand(OpenAbout, () => true)); }
        }

        private void EditConfiguration()
        {
            _navigationService.ShowConfigurationDialog(Target.EntryPoint);
        }

        private void LaunchTextureManagement()
        {
            _navigationService.ShowTextureManagementDialog(Target.EntryPoint);
        }

        private void OpenAbout()
        {
            _navigationService.ShowAboutDialog(Target.EntryPoint);
        }
    }
}