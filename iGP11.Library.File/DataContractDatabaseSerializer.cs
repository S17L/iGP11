namespace iGP11.Library.File
{
    public class DataContractDatabaseSerializer<TDatabase> : IDatabaseSerializer<TDatabase>
    {
        public TDatabase Deserialize(string database)
        {
            return database.Deserialize<TDatabase>();
        }

        public string Serialize(TDatabase database)
        {
            return database.Serialize();
        }
    }
}