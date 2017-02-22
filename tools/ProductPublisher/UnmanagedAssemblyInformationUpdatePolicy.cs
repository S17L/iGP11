using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ProductPublisher
{
    internal class UnmanagedAssemblyInformationUpdatePolicy : IUpdateAssemblyInformationPolicy
    {
        private const string PrimaryFileVersionAttributeName = "FILEVERSION";
        private const string PrimaryProductAttributeName = "PRODUCTVERSION";
        private const string SecondaryCopyrightAttributeName = "LegalCopyright";
        private const string SecondaryFileVersionAttributeName = "FileVersion";
        private const string SecondaryProductAttributeName = "ProductVersion";

        private readonly string _filePath;

        public UnmanagedAssemblyInformationUpdatePolicy(string filePath)
        {
            _filePath = filePath;
        }

        public void Update(AssemblyInformation assemblyInformation)
        {
            var lines = File.ReadAllLines(_filePath, Encoding.Unicode);

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                line = Regex.Replace(line, CreatePrimarySearchPattern(PrimaryFileVersionAttributeName), CreatePrimaryReplaceValue(PrimaryFileVersionAttributeName, ConvertVersion(assemblyInformation.Version)));
                line = Regex.Replace(line, CreatePrimarySearchPattern(PrimaryProductAttributeName), CreatePrimaryReplaceValue(PrimaryProductAttributeName, ConvertVersion(assemblyInformation.Version)));
                line = Regex.Replace(line, CreateSecondarySearchPattern(SecondaryCopyrightAttributeName), CreateSecondaryReplaceValue(SecondaryCopyrightAttributeName, assemblyInformation.Copyright));
                line = Regex.Replace(line, CreateSecondarySearchPattern(SecondaryFileVersionAttributeName), CreateSecondaryReplaceValue(SecondaryFileVersionAttributeName, assemblyInformation.Version));
                line = Regex.Replace(line, CreateSecondarySearchPattern(SecondaryProductAttributeName), CreateSecondaryReplaceValue(SecondaryProductAttributeName, assemblyInformation.ProductVersion));
                lines[i] = line;
            }

            File.WriteAllLines(_filePath, lines, Encoding.Unicode);
        }

        private static string ConvertVersion(string version)
        {
            return version.Replace(".", ",");
        }

        private static string CreatePrimaryReplaceValue(string key, string value)
        {
            return $"{key} {value}";
        }

        private static string CreatePrimarySearchPattern(string key)
        {
            return $@"{key} [\d\,]+";
        }

        private static string CreateSecondaryReplaceValue(string key, string value)
        {
            return $"\"{key}\", \"{value}\"";
        }

        private static string CreateSecondarySearchPattern(string key)
        {
            return $@"[""]{key}[""], [""](?<="")(?:\\.|[^""\\])*(?="")[""]";
        }
    }
}