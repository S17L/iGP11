using System;
using System.Runtime.Serialization;

namespace iGP11.Tool.ReadModel.Api.Model
{
    [DataContract]
    public class InjectionProfile
    {
        public InjectionProfile(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        [DataMember(Name = "id", IsRequired = true)]
        public Guid Id { get; private set; }

        [DataMember(Name = "name", IsRequired = true)]
        public string Name { get; private set; }
    }
}