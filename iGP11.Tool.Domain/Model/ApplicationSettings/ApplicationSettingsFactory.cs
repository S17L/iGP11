using iGP11.Library;
using iGP11.Tool.Domain.Configuration;

namespace iGP11.Tool.Domain.Model.ApplicationSettings
{
    public class ApplicationSettingsFactory
    {
        public ApplicationSettings Create()
        {
            return Configurations.ApplicationSettings.Deserialize<ApplicationSettings>();
        }
    }
}