using System.Collections.Generic;

using iGP11.Tool.Model;
using iGP11.Tool.ViewModel;

namespace iGP11.Tool
{
    public class NavigationService : INavigationService
    {
        public void ShowAboutDialog(Target target)
        {
            new AboutWindow(target).ShowDialog();
        }

        public AddedProfile ShowAddProfileDialog(Target invoker, IEnumerable<ProfileViewModel> profiles)
        {
            var window = new AddProfileWindow(invoker, profiles);
            return window.ShowDialog().GetValueOrDefault()
                       ? new AddedProfile(window.ProfileName, window.BasedOnProfileId)
                       : null;
        }

        public void ShowConfigurationDialog(Target invoker)
        {
            new ConfigurationWindow(invoker).ShowDialog();
        }

        public bool ShowConfirmationDialog(Target invoker, string title, string question)
        {
            return new ConfirmationWindow(invoker, title, question).ShowDialog().GetValueOrDefault();
        }

        public void ShowInformationDialog(Target invoker, string title, string information)
        {
            new InformationWindow(invoker, title, information).ShowDialog();
        }

        public string ShowRenameProfileDialog(Target invoker, string name)
        {
            var window = new RenameProfileWindow(invoker, name);
            return window.ShowDialog().GetValueOrDefault()
                       ? window.ProfileName
                       : null;
        }

        public void ShowTextureManagementDialog(Target invoker)
        {
            new TextureManagementWindow(invoker).ShowDialog();
        }
    }
}