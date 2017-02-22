namespace iGP11.Tool.ViewModel.Texture
{
    public interface ITexturePreviewViewer
    {
        void Clear();

        void Enqueue(string fileName, string filePath);

        void Rebind();
    }
}