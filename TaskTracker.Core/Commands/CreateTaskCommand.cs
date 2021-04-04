using System;
using System.Threading;
using System.Threading.Tasks;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Shared.Events;

using TaskTracker.Core.Data;

using Task = TaskTracker.Core.Entities.Task;

namespace TaskTracker.Core.Commands
{
    public class CreateTaskCommand : IRequest<Task>
    {
        public string Description { get; set; }

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
                var task = new Entities.Task
                {
                    Id = Guid.NewGuid(),
                    PublicId = Guid.NewGuid().ToString(),
                    Description = command.Description,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Status = Entities.TaskStatus.Open,
                };
                await defaultContext.AddAsync(task, cancellationToken);
                await defaultContext.SaveChangesAsync(cancellationToken);

                await publishEndpoint.Publish(new TaskCreatedEvent
                {
                    EventId = Guid.NewGuid(),
                    EventName = nameof(TaskCreatedEvent),
                    EventProducer = "TaskTracker",
                    EventTime = DateTimeOffset.UtcNow,
                    TaskId = task.PublicId,
                    Description = task.Description,
                    Created = task.CreatedAt,
                    Status = (Shared.Events.TaskStatus)task.Status
                }, cancellationToken);
                return task;
            }
        }
    }
}
