using System.Collections.Generic;
using System.Threading.Tasks;

using iGP11.Tool.Shared.Model.GameSettings;

namespace iGP11.Tool.ReadModel.Api
{
    public interface IFindGamesQuery
    {
        Task<IEnumerable<Game>> FindAllAsync();
    }
}