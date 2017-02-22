namespace iGP11.Library.File
{
    internal interface IOperation
    {
        void Commit();

        void Rollback();
    }
}