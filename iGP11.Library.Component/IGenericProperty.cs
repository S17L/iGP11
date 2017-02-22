namespace iGP11.Library.Component
{
    public interface IGenericProperty<TProperty> : IProperty
    {
        TProperty Value { get; set; }
    }
}