using System;
using System.Threading.Tasks;

using Analytics.Core.Commands;

using Shared.Events;
using MassTransit;

using MediatR;

namespace Analytics.Client.Consumers
{
    public class TaskCreatedConsumer : IConsumer<TaskCreatedEvent>
    {
        private readonly IMediator mediator;

        public TaskCreatedConsumer(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Consume(ConsumeContext<TaskCreatedEvent> context)
        {
            await mediator.Send(new CreateTaskCommand()
            {
                TaskId = context.Message.TaskId,
                Created = context.Message.Created,
                Description = context.Message.Description,
                Status = (Core.Entities.TaskStatus)context.Message.Status
            });
            Console.WriteLine($"Receive event {context.Message.EventName} in {context.Message.EventProducer}");
        }
    }

}
