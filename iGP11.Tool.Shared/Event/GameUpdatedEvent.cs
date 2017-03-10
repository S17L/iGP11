﻿using System;
using System.Runtime.Serialization;

namespace iGP11.Tool.Shared.Event
{
    [DataContract]
    public class GameUpdatedEvent
    {
        public GameUpdatedEvent(Guid id, string name, string filePath)
        {
            Id = id;
            Name = name;
            FilePath = filePath;
        }

        [DataMember(Name = "id", IsRequired = true)]
        public Guid Id { get; private set; }

        [DataMember(Name = "name", IsRequired = true)]
        public string Name { get; private set; }

        [DataMember(Name = "filePath", EmitDefaultValue = true)]
        public string FilePath { get; private set; }
    }
}