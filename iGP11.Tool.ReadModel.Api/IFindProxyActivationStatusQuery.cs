using System.Threading.Tasks;

using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.ReadModel.Api
{
    public interface IFindProxyActivationStatusQuery
    {
        Task<ActivationStatus> FindActivationStatusAsync(string applicationFilePath);
    }
}