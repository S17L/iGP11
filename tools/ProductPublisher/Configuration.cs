using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ProductPublisher
{
    [DataContract]
    internal class Configuration
    {
        [DataMember(Name = "assemblies", IsRequired = true)]
        public IEnumerable<string> Assemblies { get; private set; }

        [DataMember(Name = "assemblyInformation", IsRequired = true)]
        public AssemblyInformation AssemblyInformation { get; private set; }

        [DataMember(Name = "build", IsRequired = true)]
        public string Build { get; private set; }

        [DataMember(Name = "commitId", IsRequired = true)]
        public string CommitId { get; private set; }

        [DataMember(Name = "getCommitId", IsRequired = true)]
        public string GetCommitId { get; private set; }

        [DataMember(Name = "makePackage", IsRequired = true)]
        public string MakePackage { get; private set; }

        [DataMember(Name = "root", IsRequired = true)]
        public string Root { get; private set; }
    }
}