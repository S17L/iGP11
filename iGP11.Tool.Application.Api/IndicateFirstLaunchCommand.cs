using System;
using System.Runtime.Serialization;

namespace iGP11.Tool.Application.Api
{
    [DataContract]
    public class IndicateFirstLaunchCommand
    {
        public IndicateFirstLaunchCommand(DateTime time)
        {
            Time = time;
        }

        [DataMember(Name = "time")]
        public DateTime Time { get; private set; }
    }
}