namespace iGP11.Tool.Application
{
    internal static class MapperExtensions
    {
        public static TDestination Map<TDestination>(this object source)
        {
            return Mapper.Current.Map<TDestination>(source);
        }
    }
}