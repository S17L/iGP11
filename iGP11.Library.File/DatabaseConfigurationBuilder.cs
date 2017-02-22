namespace iGP11.Library.File
{
    public class DatabaseConfigurationBuilder<TDatabase>
    {
        internal IDatabaseEncryption DatabaseEncryption { get; private set; }

        internal IDatabaseFactory<TDatabase> DatabaseFactory { get; private set; }

        internal IDatabaseSerializer<TDatabase> DatabaseSerializer { get; private set; }

        public void Configure(IDatabaseEncryption databaseEncryption)
        {
            DatabaseEncryption = databaseEncryption;
        }

        public void Configure(IDatabaseFactory<TDatabase> databaseFactory)
        {
            DatabaseFactory = databaseFactory;
        }

        public void Configure(IDatabaseSerializer<TDatabase> databaseSerializer)
        {
            DatabaseSerializer = databaseSerializer;
        }
    }
}