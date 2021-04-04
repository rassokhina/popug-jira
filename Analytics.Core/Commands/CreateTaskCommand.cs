using System;
using System.Threading;
using System.Threading.Tasks;

using Analytics.Core.Data;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Task = Analytics.Core.Entities.Task;
using TaskStatus = Analytics.Core.Entities.TaskStatus;

namespace Analytics.Core.Commands
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
                    Status = command.Status,
                };
                await defaultContext.AddAsync(task, cancellationToken);
                await defaultContext.SaveChangesAsync(cancellationToken);
                return task;
            }
        }
    }
}
