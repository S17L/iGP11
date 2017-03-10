using iGP11.Library;
using iGP11.Tool.Domain.Configuration;
using iGP11.Tool.Domain.Exceptions;

namespace iGP11.Tool.Domain.Model.GameSettings
{
    public class GameFactory
    {
        public Game Create(GameType gameType)
        {
            var template = Configurations.ResourceManager.GetString(gameType.GetResourceKey());
            if (template == null)
            {
                throw new GameTemplateNotFoundException("game template could not be found");
            }

            return template.Deserialize<Game>();
        }
    }
}