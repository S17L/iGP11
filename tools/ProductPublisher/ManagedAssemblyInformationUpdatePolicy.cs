using System.IO;
using System.Text.RegularExpressions;

namespace ProductPublisher
{
    internal class ManagedAssemblyInformationUpdatePolicy : IUpdateAssemblyInformationPolicy
    {
        private const string CopyrightAttributeName = "AssemblyCopyright";
        private const string FileVersionAttributeName = "AssemblyFileVersion";
        private const string ProductAttributeName = "AssemblyInformationalVersion";
        private const string VersionAttributeName = "AssemblyInformation";

        private readonly string _filePath;

        public ManagedAssemblyInformationUpdatePolicy(string filePath)
        {
            _filePath = filePath;
        }

        public void Update(AssemblyInformation assemblyInformation)
        {
            var lines = File.ReadAllLines(_filePath);

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                line = Regex.Replace(line, CreateSearchPattern(CopyrightAttributeName), CreateReplaceValue(CopyrightAttributeName, assemblyInformation.Copyright));
                line = Regex.Replace(line, CreateSearchPattern(FileVersionAttributeName), CreateReplaceValue(FileVersionAttributeName, assemblyInformation.Version));
                line = Regex.Replace(line, CreateSearchPattern(ProductAttributeName), CreateReplaceValue(ProductAttributeName, assemblyInformation.ProductVersion));
                line = Regex.Replace(line, CreateSearchPattern(VersionAttributeName), CreateReplaceValue(VersionAttributeName, assemblyInformation.Version));
                lines[i] = line;
            }

            File.WriteAllLines(_filePath, lines);
        }

        private static string CreateReplaceValue(string key, string value)
        {
            return $"{key}(\"{value}\")";
        }

        private static string CreateSearchPattern(string key)
        {
            return $@"{key}[(][""](?<="")(?:\\.|[^""\\])*(?="")[""][)]";
        }
    }
}