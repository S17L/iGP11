namespace iGP11.Library.Hub.Client.Action
{
    public static class EventExtensions
    {
        public static FeedbackEventContext CreateEventContext<TEvent>(this Event<TEvent> @event, IHubClientFactory hubClientFactory)
        {
            return new FeedbackEventContext(@event.NotificationRecipientId, hubClientFactory);
        }
    }
}