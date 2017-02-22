using System;
using System.Threading.Tasks;

namespace iGP11.Tool.ReadModel.Api
{
    public interface IFindFirstLaunchTimeQuery
    {
        Task<DateTime?> IsFirstRunAsync();
    }
}