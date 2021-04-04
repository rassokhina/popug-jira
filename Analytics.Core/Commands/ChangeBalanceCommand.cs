using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Analytics.Core.Data;
using Analytics.Core.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Analytics.Core.Commands
{
    public class ChangeBalanceCommand : IRequest
    {
        public string UserId { get; set; }

        public string WalletId { get; set; }

        public DateTimeOffset Time { get; set; }

        public int BalanceChange { get; set; }

        public class ChangeBalanceCommandHandler : IRequestHandler<ChangeBalanceCommand>
        {
            private readonly DefaultContext defaultContext;

            public ChangeBalanceCommandHandler(DefaultContext defaultContext)
            {
                this.defaultContext = defaultContext ?? throw new ArgumentNullException(nameof(defaultContext));
            }

            public async Task<Unit> Handle(ChangeBalanceCommand command, CancellationToken cancellationToken)
            {
                var wallet = await defaultContext.Wallets
                    .FirstOrDefaultAsync(x => x.UserId == command.UserId, cancellationToken)
                    .ConfigureAwait(false);
                if (wallet != null)
                {
                    //recalculate balance
                    wallet.Balance = wallet.Balance + command.BalanceChange;
                    wallet.ChangeTime = command.Time;
                }
                else
                {
                    wallet = new Wallet()
                    {
                        Id = Guid.NewGuid(),
                        PublicId = command.WalletId,
                        CreatedAt = DateTimeOffset.UtcNow,
                        UserId = command.UserId,
                        Balance = command.BalanceChange,
                        ChangeTime = DateTimeOffset.UtcNow
                    };
                    await defaultContext.AddAsync(wallet, cancellationToken);
                }
                await defaultContext.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}