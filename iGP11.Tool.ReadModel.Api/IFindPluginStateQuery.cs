using System.Threading.Tasks;

using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.ReadModel.Api
{
    public interface IFindPluginStateQuery
    {
        Task<ProxySettings> FindPluginStateAsync();
    }
}