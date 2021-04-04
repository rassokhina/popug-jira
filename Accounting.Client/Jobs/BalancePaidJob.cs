using System;
using System.Linq;
using System.Threading;

using Accounting.Core.Data;
using Accounting.Core.Entities;

using MassTransit;

using Microsoft.EntityFrameworkCore;

using Quartz;

using Shared.Events;

using Task = System.Threading.Tasks.Task;

namespace Accounting.Client.Jobs
{
    [DisallowConcurrentExecution]
    public class BalancePaidJob : IJob
    {
        private readonly DefaultContext defaultContext;
        private readonly IPublishEndpoint publishEndpoint;

        public BalancePaidJob(DefaultContext defaultContext, IPublishEndpoint publishEndpoint)
        {
            this.defaultContext = defaultContext ?? throw new ArgumentNullException(nameof(defaultContext));
            this.publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var wallets = await defaultContext.Wallets
                    .Include(x => x.WalletAudits)
                    .Where(x => x.Balance > 0)
                    .ToListAsync()
                    .ConfigureAwait(false);

                var utcNow = DateTimeOffset.UtcNow;
                if (wallets.Count > 0)
                {
                    wallets.ForEach(async wallet =>
                    {
                        await defaultContext.AddAsync(new WalletAudit()
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = utcNow,
                            PublicId = Guid.NewGuid().ToString(),
                            UserId = wallet.UserId,
                            WalletId = wallet.Id,
                            Description = $"Balance paid for {utcNow.Date:d}",
                            Debit = wallet.Balance,
                            Credit = 0
                        });
                        wallet.Balance = 0;
                        await defaultContext.SaveChangesAsync();

                        await publishEndpoint.Publish(new BalancePaidEvent()
                        {
                            EventId = Guid.NewGuid(),
                            EventName = nameof(BalancePaidEvent),
                            EventProducer = "Accounting",
                            EventTime = utcNow,
                            UserId = wallet.UserId,
                            WalletId = wallet.PublicId,
                            Time = utcNow,
                            Paid = wallet.Balance
                        });
                        await publishEndpoint.Publish(new BalanceChangedEvent()
                        {
                            EventId = Guid.NewGuid(),
                            EventName = nameof(BalanceChangedEvent),
                            EventProducer = "Accounting",
                            EventTime = utcNow,
                            UserId = wallet.UserId,
                            WalletId = wallet.PublicId,
                            Time = utcNow,
                            BalanceChange = -wallet.Balance
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                // handle exception
                throw;
            }
        }
    }
}
