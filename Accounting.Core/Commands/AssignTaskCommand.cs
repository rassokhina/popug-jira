using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Accounting.Core.Data;
using Accounting.Core.Entities;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Shared.Events;

namespace Accounting.Core.Commands
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
                    try
                    {
                        task.UserId = command.UserId;
                        task.UpdatedAt = command.Time;

                        var wallet = await defaultContext.Wallets
                            .Include(x => x.WalletAudits)
                            .FirstOrDefaultAsync(x => x.UserId == task.UserId, cancellationToken)
                            .ConfigureAwait(false);

                        var debit = new Random().Next(10, 20);
                        if (wallet != null)
                        {
                            await defaultContext.AddAsync(new WalletAudit()
                            {
                                Id = Guid.NewGuid(),
                                CreatedAt = command.Time,
                                PublicId = Guid.NewGuid().ToString(),
                                UserId = command.UserId,
                                WalletId = wallet.Id,
                                Description = $"Assign task \"{task.Description}\"",
                                Debit = debit,
                                Credit = 0
                            });
                            //recalculate balance
                            wallet.Balance = wallet.Balance - debit;
                            await defaultContext.SaveChangesAsync(cancellationToken);

                            await publishEndpoint.Publish(new BalanceChangedEvent()
                            {
                                EventId = Guid.NewGuid(),
                                EventName = nameof(BalanceChangedEvent),
                                EventProducer = "Accounting",
                                EventTime = DateTimeOffset.UtcNow,
                                UserId = task.UserId,
                                WalletId = wallet.PublicId,
                                Time = command.Time,
                                BalanceChange = -debit,
                            }, cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exception
                    }
                }
                return Unit.Value;
            }
        }
    }
}
