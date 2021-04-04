using System;
using System.Threading;
using System.Threading.Tasks;

using Accounting.Core.Commands;

using Shared.Events;
using MassTransit;

using MediatR;

namespace Accounting.Client.Consumers
{
    public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly IMediator mediator;

        public UserCreatedConsumer(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            await mediator.Send(new CreateUserCommand()
            {
                PublicId = context.Message.PublicId, 
                Username = context.Message.Username,
                Role = context.Message.Role
            });
            Console.WriteLine($"Receive event {context.Message.EventName} in {context.Message.EventProducer}");
        }
    }
}
