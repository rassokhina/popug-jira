using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Accounting.Core.Data;
using Accounting.Core.Dto;
using Accounting.Core.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Accounting.Core.Queries
{
    public class GetWalletAuditQuery : IRequest<IEnumerable<WalletAuditDto>>
    {
        public string UserId { get; set; }

        public class GetWalletAuditQueryHandler : IRequestHandler<GetWalletAuditQuery, IEnumerable<WalletAuditDto>>
        {
            private readonly DefaultContext defaultContext;

            public GetWalletAuditQueryHandler(DefaultContext defaultContext)
            {
                this.defaultContext = defaultContext ?? throw new ArgumentNullException(nameof(defaultContext));
            }

            public async Task<IEnumerable<WalletAuditDto>> Handle(GetWalletAuditQuery query,
                                                                  CancellationToken cancellationToken)
            {
                return await defaultContext.WalletAudits
                    .AsNoTracking()
                    .Where(t => t.UserId == query.UserId && t.CreatedAt.Date == DateTime.UtcNow.Date)
                    .OrderBy(x => x.CreatedAt)
                    .Select(t => new WalletAuditDto()
                    {
                        Id = t.Id,
                        Description = t.Description,
                        MoneyChange = t.Credit != 0 ? t.Credit: -t.Debit,
                        Time = t.CreatedAt
                    })
                    .ToListAsync(cancellationToken);
            }
        }
    }
}
