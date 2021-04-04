using System;
using System.Threading;
using System.Threading.Tasks;

using Shared.Events;
using MassTransit;

using MediatR;

namespace TaskTracker.Client.Consumers
{
    public class TaskAssignedConsumer : IConsumer<TaskAssignedEvent>
    {
        private readonly IMediator mediator;

        public TaskAssignedConsumer(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Consume(ConsumeContext<TaskAssignedEvent> context)
        {
            Console.WriteLine($"Receive event {context.Message.EventName} in {context.Message.EventProducer}");
            Console.WriteLine("Send notification about task assigned");
        }
    }
}
