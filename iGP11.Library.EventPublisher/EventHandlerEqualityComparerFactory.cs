using System.Collections.Generic;

namespace iGP11.Library.EventPublisher
{
    public class EventHandlerEqualityComparerFactory : IEventHandlerEqualityComparerFactory
    {
        public IEqualityComparer<IEventHandler<TEvent>> Create<TEvent>()
        {
            return new EventHandlerEqualityComparer<TEvent>();
        }

        private class EventHandlerEqualityComparer<TObject> : IEqualityComparer<IEventHandler<TObject>>
        {
            public bool Equals(IEventHandler<TObject> x, IEventHandler<TObject> y)
            {
                return ReferenceEquals(x, y);
            }

            public int GetHashCode(IEventHandler<TObject> obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}