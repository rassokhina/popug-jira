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
    public class FinishTaskCommand : IRequest
    {
        public string TaskId { get; set; }

        public DateTimeOffset Time { get; set; }

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
                    .FirstOrDefaultAsync(x => x.PublicId == command.TaskId, cancellationToken)
                    .ConfigureAwait(false);
                if (task != null)
                {
                    try
                    {
                        task.Status = Entities.TaskStatus.Finished;
                        task.Finished = command.Time;
                        task.UpdatedAt = command.Time;

                        var wallet = await defaultContext.Wallets
                            .Include(x => x.WalletAudits)
                            .FirstOrDefaultAsync(x => x.UserId == task.UserId, cancellationToken)
                            .ConfigureAwait(false);

                        var credit = new Random().Next(20, 40);
                        if (wallet != null)
                        {
                            await defaultContext.AddAsync(new WalletAudit()
                            {
                                Id = Guid.NewGuid(),
                                CreatedAt = command.Time,
                                PublicId = Guid.NewGuid().ToString(),
                                UserId = wallet.UserId,
                                WalletId = wallet.Id,
                                Description = $"Finish task \"{task.Description}\"",
                                Debit = 0,
                                Credit = credit,
                            });
                            //recalculate balance
                            wallet.Balance = wallet.Balance + credit;
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
                                BalanceChange = credit,
                            }, cancellationToken);
                        }
                    }
                    catch (Exception)
                    {
                        // Handle exception
                    }
                }
                return Unit.Value;
            }
        }
    }
}
