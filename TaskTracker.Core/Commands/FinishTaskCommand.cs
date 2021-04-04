using System;
using System.Threading;
using System.Threading.Tasks;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Shared.Events;

using TaskTracker.Core.Data;

namespace TaskTracker.Core.Commands
{
    public class FinishTaskCommand : IRequest
    {
        public Guid TaskId { get; set; }

        public string UserId { get; set; }

        public class FinishTaskCommandHandler : IRequestHandler<FinishTaskCommand>
        {
            private readonly DefaultContext defaultContext;
            private readonly IPublishEndpoint publishEndpoint;

            public FinishTaskCommandHandler(DefaultContext defaultContext, IPublishEndpoint publishEndpoint)
            {
                this.defaultContext = defaultContext ?? throw new ArgumentNullException(nameof(defaultContext));
                this.publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            }

            public async Task<Unit> Handle(FinishTaskCommand command, CancellationToken cancellationToken)
            {
                var task = await defaultContext.Tasks
                    .FirstOrDefaultAsync(x => x.Id == command.TaskId, cancellationToken)
                    .ConfigureAwait(false);
                if (task == null)
                {
                    throw new ArgumentNullException(nameof(task));
                }
                task.Status = Entities.TaskStatus.Finished;
                task.Finished = DateTimeOffset.UtcNow;
                task.UpdatedAt = DateTimeOffset.UtcNow;
                await defaultContext.SaveChangesAsync(cancellationToken);

                await publishEndpoint.Publish(new TaskFinishedEvent
                {
                    EventId = Guid.NewGuid(),
                    EventName = nameof(TaskFinishedEvent),
                    EventProducer = "TaskTracker",
                    EventTime = DateTimeOffset.UtcNow,
                    TaskId = task.PublicId,
                    Time = task.Finished.Value,
                }, cancellationToken);
                return Unit.Value;
            }
        }
    }
}
