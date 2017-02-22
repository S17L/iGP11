using System;
using System.Threading.Tasks;

using iGP11.Tool.Shared.Model.InjectionSettings;

namespace iGP11.Tool.ReadModel.Api
{
    public interface IFindInjectionSettingsByIdQuery
    {
        Task<InjectionSettings> FindByIdAsync(Guid id);
    }
}