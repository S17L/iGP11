namespace iGP11.Library
{
    public interface ISecuredText
    {
        string GetUnsecuredText();

        bool IsEmpty();

        bool IsEqual(ISecuredText securedText);
    }
}