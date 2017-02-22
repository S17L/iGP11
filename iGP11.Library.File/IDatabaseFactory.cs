namespace iGP11.Library.File
{
    public interface IDatabaseFactory<out TDatabase>
    {
        TDatabase Create();
    }
}