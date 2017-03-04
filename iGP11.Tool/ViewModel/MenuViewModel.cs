using iGP11.Tool.Common;
using iGP11.Tool.Model;

namespace iGP11.Tool.ViewModel
{
    public sealed class MenuViewModel : ViewModel
    {
        private readonly INavigationService _navigationService;

        public MenuViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            EditConfigurationCommand = new ActionCommand(EditConfiguration, () => true);
            LaunchTextureManagementCommand = new ActionCommand(LaunchTextureManagement, () => true);
            OpenAboutCommand = new ActionCommand(OpenAbout, () => true);
        }

        public IActionCommand EditConfigurationCommand { get; }

        public IActionCommand LaunchTextureManagementCommand { get; }

        public IActionCommand OpenAboutCommand { get; }

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