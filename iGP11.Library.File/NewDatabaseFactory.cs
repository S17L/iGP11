namespace iGP11.Library.File
{
    public class NewDatabaseFactory<TDatabase> : IDatabaseFactory<TDatabase>
        where TDatabase : class, new()
    {
        public TDatabase Create()
        {
            return new TDatabase();
        }
    }
}