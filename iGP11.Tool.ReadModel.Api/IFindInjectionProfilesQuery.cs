using System.Collections.Generic;
using System.Threading.Tasks;

using iGP11.Tool.ReadModel.Api.Model;

namespace iGP11.Tool.ReadModel.Api
{
    public interface IFindInjectionProfilesQuery
    {
        Task<IEnumerable<InjectionProfile>> FindAllAsync();
    }
}