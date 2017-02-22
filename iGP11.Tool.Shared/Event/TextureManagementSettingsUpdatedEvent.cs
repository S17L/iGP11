using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model.TextureManagementSettings;

namespace iGP11.Tool.Shared.Event
{
    [DataContract]
    public class TextureManagementSettingsUpdatedEvent
    {
        public TextureManagementSettingsUpdatedEvent(TextureManagementSettings settings)
        {
            Settings = settings;
        }

        [DataMember(Name = "settings", IsRequired = true)]
        public TextureManagementSettings Settings { get; private set; }
    }
}