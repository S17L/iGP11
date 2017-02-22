namespace iGP11.Library.Component
{
    public interface IPropertyConverter<TProperty>
    {
        TProperty ConvertFrom(object value);

        object ConvertTo(TProperty value);

        bool IsValid(object value);
    }
}