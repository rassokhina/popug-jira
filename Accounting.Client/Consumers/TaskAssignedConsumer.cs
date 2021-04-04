using System;
using System.Threading;
using System.Threading.Tasks;

using Accounting.Core.Commands;

using Shared.Events;
using MassTransit;

using MediatR;

namespace Accounting.Client.Consumers
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
            await mediator.Send(new AssigneTaskCommand()
            {
                TaskId = context.Message.TaskId,
                Time = context.Message.Time,
                UserId = context.Message.UserId
            });
            Console.WriteLine($"Receive event {context.Message.EventName} in {context.Message.EventProducer}");
        }
    }
}
