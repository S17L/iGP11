using System.Threading.Tasks;

namespace iGP11.Tool.Domain.Model.ApplicationSettings
{
    public interface IApplicationSettingsRepository
    {
        Task<ApplicationSettings> LoadAsync();

        Task SaveAsync(ApplicationSettings applicationSettings);
    }
}