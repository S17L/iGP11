using System;

using iGP11.Library.EventPublisher;
using iGP11.Library.Network;
using iGP11.Tool.Events;
using iGP11.Tool.Model;

namespace iGP11.Tool
{
    public class ApplicationActionCommandHandler : INetworkCommandHandler
    {
        private readonly IEventPublisher _eventPublisher;

        public ApplicationActionCommandHandler(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public bool Handle(Command command, ref CommandOutput output)
        {
            if (command.Name != CommandId.ApplicationActionCommand)
            {
                return false;
            }

            _eventPublisher.PublishAsync(new ApplicationActionEvent((ApplicationAction)Enum.Parse(typeof(ApplicationAction), command.Data))).Wait();
            output = new CommandOutput(null);

            return true;
        }
    }
}