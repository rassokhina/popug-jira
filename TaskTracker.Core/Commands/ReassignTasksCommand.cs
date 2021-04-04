using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Shared.Events;

using TaskTracker.Core.Data;

namespace TaskTracker.Core.Commands
{
    public class ReassignTasksCommand : IRequest
    {
        public class ReassignTasksCommandHandler : IRequestHandler<ReassignTasksCommand>
        {
            private readonly DefaultContext defaultContext;
            private readonly IPublishEndpoint publishEndpoint;

            public ReassignTasksCommandHandler(DefaultContext defaultContext, IPublishEndpoint publishEndpoint)
            {
                this.defaultContext = defaultContext ?? throw new ArgumentNullException(nameof(defaultContext));
                this.publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            }

            public async Task<Unit> Handle(ReassignTasksCommand command, CancellationToken cancellationToken)
            {
                var tasks = await defaultContext.Tasks
                    .Where(x => x.Status == Entities.TaskStatus.Open)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
                var userIds = await defaultContext.Users.Select(x => x.PublicId).ToArrayAsync(cancellationToken).ConfigureAwait(false);

                var events = new List<TaskAssignedEvent>();
                if (userIds.Length > 0)
                {
                    Random rand = new Random();
                    tasks.ForEach(t => {
                        int position = rand.Next(userIds.Length);
                        t.UserId = userIds[position];
                        events.Add(new TaskAssignedEvent()
                        {
                            EventId = Guid.NewGuid(),
                            EventName = nameof(TaskAssignedEvent),
                            EventProducer = "TaskTracker",
                            EventTime = DateTimeOffset.UtcNow,
                            TaskId = t.PublicId,
                            Time = DateTimeOffset.UtcNow,
                            UserId = t.UserId
                        });
                    });
                    await defaultContext.SaveChangesAsync(cancellationToken);

                    foreach (var evt in events)
                    {
                        await publishEndpoint.Publish(evt, cancellationToken);
                    }
                }
                return Unit.Value;
            }
        }
    }
}
