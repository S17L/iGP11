namespace iGP11.Tool.Application
{
    public class Mapper
    {
        private static Mapper _current;

        public static Mapper Current
        {
            get { return _current ?? (_current = new Mapper()); }
            set { _current = value; }
        }

        public virtual TSource Clone<TSource>(TSource source)
        {
            return default(TSource);
        }

        public virtual TDestination Map<TDestination>(object source)
        {
            return default(TDestination);
        }
    }
}