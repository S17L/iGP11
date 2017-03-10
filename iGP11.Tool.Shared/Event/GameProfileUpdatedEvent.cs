using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model.GameSettings;

namespace iGP11.Tool.Shared.Event
{
    [DataContract]
    public class GameProfileUpdatedEvent
    {
        public GameProfileUpdatedEvent(GameProfile gameProfile)
        {
            GameProfile = gameProfile;
        }

        [DataMember(Name = "gameProfile", IsRequired = true)]
        public GameProfile GameProfile { get; private set; }
    }
}