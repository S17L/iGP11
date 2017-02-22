namespace iGP11.Tool.Domain.Model.Directory
{
    public interface IFileContentLoadingPolicy
    {
        byte[] Load();
    }
}