﻿using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Shared.Event;

namespace iGP11.Tool.ReadModel.EventHandler
{
    public class ApplicationStartedEventHandler : IDomainEventHandler<GameStartedEvent>
    {
        private readonly InMemoryDatabase _database;

        public ApplicationStartedEventHandler(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task HandleAsync(DomainEventContext context, GameStartedEvent @event)
        {
            _database.InjectionStatuses[@event.FilePath] = @event.Status;
            await context.EmitAsync(new Shared.Notification.ApplicationStartedNotification(@event.Status));
        }
    }
}