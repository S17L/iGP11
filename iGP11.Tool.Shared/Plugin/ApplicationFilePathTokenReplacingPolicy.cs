using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using iGP11.Library;
using iGP11.Library.Component;

namespace iGP11.Tool.Shared.Plugin
{
    public class ApplicationFilePathTokenReplacingPolicy : ITokenReplacingPolicy
    {
        private const string KeyApplicationDirectoryPath = "<# app_dir #>";
        private const string KeyApplicationFilePath = "<# app_file #>";

        private readonly Func<string> _provider;

        public ApplicationFilePathTokenReplacingPolicy(Func<string> provider)
        {
            _provider = provider;
        }

        public string Apply(string expression)
        {
            var filePath = _provider();
            var directoryPath = filePath.IsNotNullOrEmpty()
                                    ? Path.GetDirectoryName(filePath)
                                    : string.Empty;

            var dictionary = new Dictionary<string, string>
            {
                [KeyApplicationFilePath] = filePath,
                [KeyApplicationDirectoryPath] = directoryPath
            };

            return dictionary.Aggregate(expression, (current, pair) => current.Replace(pair.Key, pair.Value));
        }
    }
}