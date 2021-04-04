using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Accounting.Core.Data;
using MediatR;

using Microsoft.EntityFrameworkCore;

using TaskStatus = Accounting.Core.Entities.TaskStatus;

namespace Accounting.Core.Queries
{
    public class GetEarnedByManagementQuery : IRequest<int>
    {
        public string UserId { get; set; }

        public class GetEarnedByManagementQueryHandler : IRequestHandler<GetEarnedByManagementQuery, int>
        {
            private readonly DefaultContext defaultContext;

            public GetEarnedByManagementQueryHandler(DefaultContext defaultContext)
            {
                this.defaultContext = defaultContext ?? throw new ArgumentNullException(nameof(defaultContext));
            }

            public async Task<int> Handle(GetEarnedByManagementQuery query,
                                                                  CancellationToken cancellationToken)
            {
                return await defaultContext.Tasks
                           .AsNoTracking()
                           .Where(t => t.CreatedAt.Date == DateTime.UtcNow.Date && t.Status == TaskStatus.Open)
                           .SumAsync(x => x.Price, cancellationToken) -
                       await defaultContext.Tasks
                           .AsNoTracking()
                           .Where(t => t.Finished.HasValue && t.Finished.Value.Date == DateTime.UtcNow.Date && t.Status == TaskStatus.Finished)
                           .SumAsync(x => x.Price, cancellationToken);
            }
        }
    }
}
