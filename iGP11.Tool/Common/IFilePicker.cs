namespace iGP11.Tool.Common
{
    public interface IFilePicker
    {
        void OpenDirectory(string filePath);

        void OpenFile(string filePath);

        string Pick(string initialFilePath);
    }
}