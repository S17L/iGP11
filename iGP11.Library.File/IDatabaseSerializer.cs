namespace iGP11.Library.File
{
    public interface IDatabaseSerializer<TDatabase>
    {
        TDatabase Deserialize(string database);

        string Serialize(TDatabase database);
    }
}