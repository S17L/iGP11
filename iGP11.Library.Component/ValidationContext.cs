using System;
using System.IO;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Library.Component
{
    public class ValidationContext : IValidationContext
    {
        public bool IsDirectoryPathValid(string directoryPath)
        {
            Uri uri;
            return directoryPath.IsNotNullOrEmpty()
                   && Uri.TryCreate(directoryPath, UriKind.Absolute, out uri);
        }

        public bool IsFilePathValid(string filePath)
        {
            Uri uri;
            return filePath.IsNotNullOrEmpty()
                   && Uri.TryCreate(filePath, UriKind.Absolute, out uri)
                   && File.Exists(filePath);
        }
    }
}