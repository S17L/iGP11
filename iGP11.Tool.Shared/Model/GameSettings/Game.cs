using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model.GameSettings
{
    [DataContract]
    public class Game
    {
        [DataMember(Name = "id", IsRequired = true)]
        public Guid Id { get; set; }

        [DataMember(Name = "name", IsRequired = true)]
        [Editable]
        public string Name { get; set; }

        [DataMember(Name = "filePath", EmitDefaultValue = true)]
        [Editable]
        [FilePath]
        public string FilePath { get; set; }

        [DataMember(Name = "profileId")]
        [Editable]
        public Guid ProfileId { get; set; }

        [DataMember(Name = "profiles", IsRequired = true)]
        [Editable]
        public List<GameProfile> Profiles { get; set; }
    }
}
