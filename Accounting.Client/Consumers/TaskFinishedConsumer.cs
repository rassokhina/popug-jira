using System;
using System.Threading;
using System.Threading.Tasks;

using Accounting.Core.Commands;

using Shared.Events;
using MassTransit;

using MediatR;

namespace Accounting.Client.Consumers
{
    public class TaskFinishedConsumer : IConsumer<TaskFinishedEvent>
    {
        private readonly IMediator mediator;

        public TaskFinishedConsumer(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Consume(ConsumeContext<TaskFinishedEvent> context)
        {
            await mediator.Send(new FinishTaskCommand()
            {
                TaskId = context.Message.TaskId,
                Time = context.Message.Time
            });
            Console.WriteLine($"Receive event {context.Message.EventName} in {context.Message.EventProducer}");
        }
    }
}
