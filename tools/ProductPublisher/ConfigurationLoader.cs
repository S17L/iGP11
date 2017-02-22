using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ProductPublisher
{
    internal static class ConfigurationLoader
    {
        private const string ConfigurationFileName = "configuration.json";

        internal static Configuration Load()
        {
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(File.ReadAllText(ConfigurationFileName))))
            {
                return (Configuration)new DataContractJsonSerializer(typeof(Configuration)).ReadObject(stream);
            }
        }
    }
}