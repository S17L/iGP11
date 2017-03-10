using System;
using System.Runtime.Serialization;

using iGP11.Library;

namespace iGP11.Tool.Shared.Notification
{
    [DataContract]
    public class ErrorOccuredNotification
    {
        public ErrorOccuredNotification()
        {
        }

        public ErrorOccuredNotification(Localizable error)
        {
            Error = error;
        }

        public ErrorOccuredNotification(Exception exception)
        {
            Error = Localizable.NotLocalizable(exception.Message);
            Type = Localizable.NotLocalizable(exception.GetType().FullName);
        }

        [DataMember(Name = "error")]
        public Localizable Error { get; private set; }

        [DataMember(Name = "type")]
        public Localizable Type { get; private set; }
    }
}