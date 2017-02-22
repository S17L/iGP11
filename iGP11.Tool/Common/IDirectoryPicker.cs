namespace iGP11.Tool.Common
{
    public interface IDirectoryPicker
    {
        void Open(string directory);

        string Pick(string initialDirectory);
    }
}