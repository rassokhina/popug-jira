using System;
using System.Threading.Tasks;

using Analytics.Core.Commands;

using Shared.Events;
using MassTransit;

using MediatR;

namespace Analytics.Client.Consumers
{
    public class TaskUpdatedConsumer : IConsumer<TaskUpdatedEvent>
    {
        private readonly IMediator mediator;

        public TaskUpdatedConsumer(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Consume(ConsumeContext<TaskUpdatedEvent> context)
        {
            await mediator.Send(new UpdateTaskCommand()
            {
                TaskId = context.Message.TaskId,
                Price = context.Message.Price,
            });
            Console.WriteLine($"Receive event {context.Message.EventName} in {context.Message.EventProducer}");
        }
    }

}
