using System.Collections.Generic;

using iGP11.Tool.Model;
using iGP11.Tool.ViewModel;

namespace iGP11.Tool
{
    public interface INavigationService
    {
        void ShowAboutDialog(Target target);

        AddedProfile ShowAddProfileDialog(Target invoker, IEnumerable<LookupViewModel> profiles);

        void ShowConfigurationDialog(Target invoker);

        bool ShowConfirmationDialog(Target invoker, string title, string question);

        void ShowInformationDialog(Target invoker, string title, string information);

        string ShowRenameProfileDialog(Target invoker, string name);

        void ShowTextureManagementDialog(Target invoker);
    }
}