using System;
using System.Threading;
using System.Threading.Tasks;

using Accounting.Core.Data;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Shared.Events;

using Task = Accounting.Core.Entities.Task;
using TaskStatus = Shared.Events.TaskStatus;

namespace Accounting.Core.Commands
{
    public class CreateTaskCommand : IRequest<Task>
    {
        public string TaskId { get; set; }

        public DateTimeOffset Created { get; set; }

        public string Description { get; set; }

        public TaskStatus Status { get; set; }

        public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Task>
        {
            private readonly DefaultContext defaultContext;
            private readonly IPublishEndpoint publishEndpoint;
            public CreateTaskCommandHandler(DefaultContext defaultContext, IPublishEndpoint publishEndpoint)
            {
                this.defaultContext = defaultContext ?? throw new ArgumentNullException(nameof(defaultContext));
                this.publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            }

            public async Task<Task> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
            {
                var task = await defaultContext.Tasks
                    .FirstOrDefaultAsync(x => x.PublicId == command.TaskId, cancellationToken)
                    .ConfigureAwait(false);
                if (task != null)
                {
                    return task;
                }
                task = new Entities.Task
                {
                    Id = Guid.NewGuid(),
                    PublicId = command.TaskId,
                    Description = command.Description,
                    CreatedAt = command.Created,
                    Status = (Entities.TaskStatus)command.Status,
                    // set task price
                    Price = new Random().Next(20, 40)
                };
                await defaultContext.AddAsync(task, cancellationToken);
                await defaultContext.SaveChangesAsync(cancellationToken);

                await publishEndpoint.Publish(new TaskUpdatedEvent
                {
                    EventId = Guid.NewGuid(),
                    EventName = nameof(TaskUpdatedEvent),
                    EventProducer = "Accounting",
                    EventTime = DateTimeOffset.UtcNow,
                    TaskId = task.PublicId,
                    Price = task.Price
                }, cancellationToken);
                return task;
            }
        }
    }
}
