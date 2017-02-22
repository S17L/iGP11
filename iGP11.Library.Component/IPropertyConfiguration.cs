namespace iGP11.Library.Component
{
    public interface IPropertyConfiguration
    {
        Localizable GroupedBy { get; }

        bool HasDisabledValidation { get; }

        bool IsAccessible { get; }

        bool IsDirectoryPath { get; }

        bool IsEditable { get; }

        bool IsTokenizable { get; }

        Localizable LongDescription { get; }

        int? Order { get; }

        Localizable ShortDescription { get; }
    }
}