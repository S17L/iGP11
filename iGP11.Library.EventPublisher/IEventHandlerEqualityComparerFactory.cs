using System.Collections.Generic;

namespace iGP11.Library.EventPublisher
{
    public interface IEventHandlerEqualityComparerFactory
    {
        IEqualityComparer<IEventHandler<TEvent>> Create<TEvent>();
    }
}