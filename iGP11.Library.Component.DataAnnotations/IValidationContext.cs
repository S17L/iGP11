namespace iGP11.Library.Component.DataAnnotations
{
    public interface IValidationContext
    {
        bool IsDirectoryPathValid(string directoryPath);

        bool IsFilePathValid(string filePath);
    }
}