using System;
using System.Runtime.Serialization;

using iGP11.Library;

namespace iGP11.Tool.Shared.Notification
{
    [DataContract]
    public class ErrorOccuredEvent
    {
        public ErrorOccuredEvent()
        {
        }

        public ErrorOccuredEvent(Localizable error)
        {
            Error = error;
        }

        public ErrorOccuredEvent(Exception exception)
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