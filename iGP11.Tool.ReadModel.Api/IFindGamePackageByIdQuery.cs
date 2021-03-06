﻿using System;
using System.Threading.Tasks;

using iGP11.Tool.Shared.Model.GameSettings;

namespace iGP11.Tool.ReadModel.Api
{
    public interface IFindGamePackageByIdQuery
    {
        Task<GamePackage> FindByGameIdAsync(Guid gameId);

        Task<GamePackage> FindByGameProfileIdAsync(Guid gameProfileId);
    }
}