using System;

using Autofac;

namespace iGP11.Tool.Bootstrapper.Autofac
{
    public class AutofacResolver : DependencyResolver
    {
        private readonly IContainer _container;

        public AutofacResolver(IContainer container)
        {
            _container = container;
        }

        public override TObject Resolve<TObject>()
        {
            return _container.Resolve<TObject>();
        }

        public override object Resolve(Type type)
        {
            return _container.Resolve(type);
        }
    }
}