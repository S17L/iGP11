using AutoMapper;

using iGP11.Tool.Shared.Model;
using iGP11.Tool.Shared.Model.ApplicationSettings;
using iGP11.Tool.Shared.Model.GameSettings;
using iGP11.Tool.Shared.Model.TextureManagementSettings;

using Mapper = iGP11.Tool.Application.Mapper;

namespace iGP11.Tool.Bootstrapper.AutoMapper
{
    internal class AutoMapperAdapter : Mapper
    {
        private static readonly IMapper _mapper;

        static AutoMapperAdapter()
        {
            _mapper = new MapperConfiguration(
                configuration =>
                {
                    CreateMapping<ApplicationSettings, Domain.Model.ApplicationSettings.ApplicationSettings>(configuration);
                    CreateMapping<BokehDoF, Domain.Model.GameSettings.BokehDoF>(configuration);
                    CreateMapping<DepthBuffer, Domain.Model.GameSettings.DepthBuffer>(configuration);
                    CreateMapping<Direct3D11PluginSettings, Domain.Model.GameSettings.Direct3D11PluginSettings>(configuration);
                    CreateMapping<Direct3D11Settings, Domain.Model.GameSettings.Direct3D11Settings>(configuration);
                    CreateMapping<Game, Domain.Model.GameSettings.Game>(configuration);
                    CreateMapping<GameProfile, Domain.Model.GameSettings.GameProfile>(configuration);
                    CreateMapping<LumaSharpen, Domain.Model.GameSettings.LumaSharpen>(configuration);
                    CreateMapping<ProxyPluginSettings, Application.Model.ProxyPluginSettings>(configuration);
                    CreateMapping<ProxySettings, Application.Model.ProxySettings>(configuration);
                    CreateMapping<TextureConversionSettings, Domain.Model.TextureManagementSettings.TextureConversionSettings>(configuration);
                    CreateMapping<TextureManagementSettings, Domain.Model.TextureManagementSettings.TextureManagementSettings>(configuration);
                    CreateMapping<Textures, Domain.Model.GameSettings.Textures>(configuration);
                    CreateMapping<Tonemap, Domain.Model.GameSettings.Tonemap>(configuration);
                    CreateMapping<UsageStatistics, Domain.Model.UsageStatistics.UsageStatistics>(configuration);
                    CreateMapping<Vibrance, Domain.Model.GameSettings.Vibrance>(configuration);
                }).CreateMapper();
        }

        public override TSource Clone<TSource>(TSource source)
        {
            return _mapper.Map<TSource>(source);
        }

        public override TDestination Map<TDestination>(object source)
        {
            return _mapper.Map<TDestination>(source);
        }

        private static void CreateMapping<TSource, TDestination>(IProfileExpression configuration)
        {
            configuration.CreateMap<TSource, TDestination>();
            configuration.CreateMap<TDestination, TSource>();
            configuration.CreateMap<TSource, TSource>();
            configuration.CreateMap<TDestination, TDestination>();
        }
    }
}