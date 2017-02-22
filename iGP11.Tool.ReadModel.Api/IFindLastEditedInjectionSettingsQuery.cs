using System.Threading.Tasks;

using iGP11.Tool.Shared.Model.InjectionSettings;

namespace iGP11.Tool.ReadModel.Api
{
    public interface IFindLastEditedInjectionSettingsQuery
    {
        Task<InjectionSettings> FindAsync();
    }
}