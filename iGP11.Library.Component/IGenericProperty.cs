namespace iGP11.Library.Component
{
    public interface IGenericProperty<TProperty> : IProperty
    {
        TProperty FormattedValue { get; }

        TProperty Value { get; set; }
    }
}