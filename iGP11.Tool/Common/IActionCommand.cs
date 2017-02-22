using System.Windows.Input;

namespace iGP11.Tool.Common
{
    public interface IActionCommand : ICommand
    {
        void Rebind();
    }
}