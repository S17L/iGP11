using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model.GameSettings;

namespace iGP11.Tool.Application.Api
{
    [DataContract]
    public class UpdateGameProfileCommand
    {
        public UpdateGameProfileCommand(GameProfile gameProfile)
        {
            GameProfile = gameProfile;
        }

        [DataMember(Name = "gameProfile")]
        public GameProfile GameProfile { get; private set; }
    }
}