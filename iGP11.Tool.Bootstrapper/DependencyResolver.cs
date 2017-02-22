using System;

namespace iGP11.Tool.Bootstrapper
{
    public class DependencyResolver
    {
        private static DependencyResolver _current;

        public static DependencyResolver Current
        {
            get { return _current ?? (_current = new DependencyResolver()); }
            set { _current = value; }
        }

        public virtual TObject Resolve<TObject>()
        {
            return default(TObject);
        }

        public virtual object Resolve(Type type)
        {
            return default(object);
        }
    }
}