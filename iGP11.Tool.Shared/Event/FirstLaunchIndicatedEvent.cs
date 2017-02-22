using System;
using System.Runtime.Serialization;

namespace iGP11.Tool.Shared.Event
{
    [DataContract]
    public class FirstLaunchIndicatedEvent
    {
        public FirstLaunchIndicatedEvent(DateTime time)
        {
            Time = time;
        }

        [DataMember(Name = "time", IsRequired = true)]
        public DateTime Time { get; private set; }
    }
}