using iGP11.Library.Component;

namespace iGP11.Tool.Framework
{
    public class DirectoryPathPropertyDataType : PropertyDataTypeAttribute
    {
        public DirectoryPathPropertyDataType()
            : base(typeof(string))
        {
        }

        public override bool IsApplicable(IPropertyConfiguration configuration)
        {
            return configuration.IsDirectoryPath;
        }
    }
}