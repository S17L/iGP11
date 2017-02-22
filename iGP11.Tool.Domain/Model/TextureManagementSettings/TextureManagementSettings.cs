using System.Runtime.Serialization;

using iGP11.Library.DDD;

namespace iGP11.Tool.Domain.Model.TextureManagementSettings
{
    [DataContract]
    public class TextureManagementSettings : AggregateRoot<AggregateId>
    {
        public TextureManagementSettings(
            AggregateId id,
            string sourceDirectory,
            string destinationDirectory,
            TextureConversionSettings multiConversion,
            TextureConversionSettings singleConversion)
            : base(id)
        {
            SourceDirectory = sourceDirectory;
            DestinationDirectory = destinationDirectory;
            MultiConversion = multiConversion;
            SingleConversion = singleConversion;
        }

        [DataMember(Name = "destinationDirectory", EmitDefaultValue = true)]
        public string DestinationDirectory { get; set; }

        [DataMember(Name = "multiConversion", EmitDefaultValue = true, IsRequired = true)]
        public TextureConversionSettings MultiConversion { get; set; }

        [DataMember(Name = "singleConversion", EmitDefaultValue = true, IsRequired = true)]
        public TextureConversionSettings SingleConversion { get; set; }

        [DataMember(Name = "sourceDirectory", EmitDefaultValue = true)]
        public string SourceDirectory { get; set; }
    }
}