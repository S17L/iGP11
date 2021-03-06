﻿using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Shared.Event
{
    [DataContract]
    public class GameStartedEvent
    {
        public GameStartedEvent(string filePath, GameLaunchingStatus status)
        {
            FilePath = filePath;
            Status = status;
        }

        [DataMember(Name = "filePath", IsRequired = true)]
        public string FilePath { get; private set; }

        [DataMember(Name = "status", IsRequired = true)]
        public GameLaunchingStatus Status { get; private set; }
    }
}