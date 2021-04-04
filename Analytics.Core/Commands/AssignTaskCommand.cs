using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Analytics.Core.Data;
using Analytics.Core.Entities;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Analytics.Core.Commands
{
    public class AssigneTaskCommand : IRequest
    {
        public string TaskId { get; set; }

        public string UserId { get; set; }

        public DateTimeOffset Time { get; set; }

        public class AssigneTaskCommandHandler : IRequestHandler<AssigneTaskCommand>
        {
            private readonly DefaultContext defaultContext;
            private readonly IPublishEndpoint publishEndpoint;

            public AssigneTaskCommandHandler(DefaultContext defaultContext, IPublishEndpoint publishEndpoint)
            {
                this.defaultContext = defaultContext ?? throw new ArgumentNullException(nameof(defaultContext));
                this.publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            }

            public async Task<Unit> Handle(AssigneTaskCommand command, CancellationToken cancellationToken)
            {
                var task = await defaultContext.Tasks
                    .FirstOrDefaultAsync(x => x.PublicId == command.TaskId, cancellationToken)
                    .ConfigureAwait(false);
                if (task != null)
                {
                    task.UserId = command.UserId;
                    task.UpdatedAt = command.Time;
                    await defaultContext.SaveChangesAsync(cancellationToken);
                }
                return Unit.Value;
            }
        }
    }
}
