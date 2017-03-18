using System.Runtime.Serialization;

namespace iGP11.Tool.Shared.Model.GameSettings
{
    [DataContract]
    public class GamePackage
    {
        public GamePackage(Game game, GameProfile gameProfile)
        {
            Game = game;
            GameProfile = gameProfile;
        }

        [DataMember(Name = "game", IsRequired = true)]
        public Game Game { get; private set; }

        [DataMember(Name = "gameProfile", IsRequired = true)]
        public GameProfile GameProfile { get; private set; }
    }
}