using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.ViewModel.Texture
{
    public interface ITextureManagementViewModel
    {
        Srgb ColorSpace { get; }

        void Rebind();
    }
}