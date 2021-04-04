using System;
using System.Threading.Tasks;

using Accounting.Core.Commands;

using Shared.Events;
using MassTransit;

using MediatR;

namespace Accounting.Client.Consumers
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
                Status = context.Message.Status
            });
        }
    }

}
