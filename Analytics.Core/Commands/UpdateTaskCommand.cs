using System;
using System.Threading;
using System.Threading.Tasks;

using Analytics.Core.Data;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Analytics.Core.Commands
{
    public class UpdateTaskCommand : IRequest
    {
        public string TaskId { get; set; }

        public int Price { get; set; }

        public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand>
        {
            private readonly DefaultContext defaultContext;
            public UpdateTaskCommandHandler(DefaultContext defaultContext, IPublishEndpoint publishEndpoint)
            {
                this.defaultContext = defaultContext ?? throw new ArgumentNullException(nameof(defaultContext));
            }

            public async Task<Unit> Handle(UpdateTaskCommand command, CancellationToken cancellationToken)
            {
                var task = await defaultContext.Tasks
                    .FirstOrDefaultAsync(x => x.PublicId == command.TaskId, cancellationToken)
                    .ConfigureAwait(false);
                if (task != null)
                {
                    task.Price = command.Price;
                    await defaultContext.SaveChangesAsync(cancellationToken);
                }
                return Unit.Value;
            }
        }
    }
}
