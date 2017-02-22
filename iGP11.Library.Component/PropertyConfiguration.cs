namespace iGP11.Library.Component
{
    public class PropertyConfiguration : IPropertyConfiguration
    {
        public Localizable GroupedBy { get; internal set; }

        public bool HasDisabledValidation { get; internal set; }

        public bool IsAccessible { get; internal set; }

        public bool IsDirectoryPath { get; internal set; }

        public bool IsEditable { get; internal set; }

        public bool IsTokenizable { get; internal set; }

        public Localizable LongDescription { get; internal set; }

        public int? Order { get; internal set; }

        public Localizable ShortDescription { get; internal set; }
    }
}