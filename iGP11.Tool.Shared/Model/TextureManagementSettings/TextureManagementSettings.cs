using System;
using System.Runtime.Serialization;

namespace iGP11.Tool.Shared.Model.TextureManagementSettings
{
    [DataContract]
    public class TextureManagementSettings
    {
        [DataMember(Name = "destinationDirectory", EmitDefaultValue = true)]
        public string DestinationDirectory { get; set; }

        [DataMember(Name = "id", EmitDefaultValue = true)]
        public Guid Id { get; set; }

        [DataMember(Name = "multiConversion", EmitDefaultValue = true, IsRequired = true)]
        public TextureConversionSettings MultiConversion { get; set; }

        [DataMember(Name = "singleConversion", EmitDefaultValue = true, IsRequired = true)]
        public TextureConversionSettings SingleConversion { get; set; }

        [DataMember(Name = "sourceDirectory", EmitDefaultValue = true)]
        public string SourceDirectory { get; set; }
    }
}