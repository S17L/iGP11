using System.Threading.Tasks;

namespace iGP11.Library.Hub.Client.Action
{
    public interface INotificationHandler<in TEvent>
    {
        Task HandleAsync(NotificationContext context, TEvent @event);
    }
}