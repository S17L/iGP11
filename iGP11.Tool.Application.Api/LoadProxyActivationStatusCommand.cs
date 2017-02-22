using System.Runtime.Serialization;

namespace iGP11.Tool.Application.Api
{
    [DataContract]
    public class LoadProxyActivationStatusCommand
    {
        public LoadProxyActivationStatusCommand(string applicationFilePath)
        {
            ApplicationFilePath = applicationFilePath;
        }

        [DataMember(Name = "applicationFilePath", IsRequired = true)]
        public string ApplicationFilePath { get; private set; }
    }
}