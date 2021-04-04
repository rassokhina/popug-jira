using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Analytics.Core.Data;
using Analytics.Core.Dto;
using Analytics.Core.Extensions;

using MassTransit.Initializers;

using MediatR;

using Microsoft.EntityFrameworkCore;

using TaskStatus = Analytics.Core.Entities.TaskStatus;

namespace Analytics.Core.Queries
{
    public class GetMostExpensiveTaskQuery : IRequest<IEnumerable<ExpensiveTaskDto>>
    {
        public class GetMostExpensiveTaskQueryHandler : IRequestHandler<GetMostExpensiveTaskQuery, IEnumerable<ExpensiveTaskDto>>
        {
            private readonly DefaultContext defaultContext;

            public GetMostExpensiveTaskQueryHandler(DefaultContext defaultContext)
            {
                this.defaultContext = defaultContext ?? throw new ArgumentNullException(nameof(defaultContext));
            }

            public async Task<IEnumerable<ExpensiveTaskDto>> Handle(GetMostExpensiveTaskQuery query,
                                                                    CancellationToken cancellationToken)
            {
                var result = new List<ExpensiveTaskDto>();
                var today = DateTimeOffset.UtcNow.Date;
                result.Add(await defaultContext.Tasks
                    .AsNoTracking()
                    .Where(t => t.Finished.HasValue && t.Finished.Value.Date == today && t.Status == TaskStatus.Finished)
                    .OrderByDescending(x => x.Price)
                    .FirstOrDefaultAsync(cancellationToken)
                    .Select(x => new ExpensiveTaskDto()
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Price = x.Price,
                        Period = PeriodDto.Day,
                        PeriodString = today.ToString("d")
                    }));

                var vv = today.FirstDayOfWeek();
                var tt = today.LastDayOfWeek();
                result.Add(await defaultContext.Tasks
                    .AsNoTracking()
                    .Where(t => t.Finished.HasValue && t.Finished.Value.Date >= today.FirstDayOfWeek() 
                                                    && t.Finished.Value.Date <= today.LastDayOfWeek() 
                                                    && t.Status == TaskStatus.Finished)
                    .OrderByDescending(x => x.Price)
                    .FirstOrDefaultAsync(cancellationToken)
                    .Select(x => new ExpensiveTaskDto()
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Price = x.Price,
                        Period = PeriodDto.Week,
                        PeriodString = today.FirstDayOfWeek().ToString("d") + " - " + today.LastDayOfWeek().ToString("d")
                    }));

                result.Add(await defaultContext.Tasks
                    .AsNoTracking()
                    .Where(t => t.Finished.HasValue && t.Finished.Value.Date >= today.FirstDayOfMonth()
                                                    && t.Finished.Value.Date <= today.LastDayOfMonth()
                                                    && t.Status == TaskStatus.Finished)
                    .OrderByDescending(x => x.Price)
                    .FirstOrDefaultAsync(cancellationToken)
                    .Select(x => new ExpensiveTaskDto()
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Price = x.Price,
                        Period = PeriodDto.Month,
                        PeriodString = today.FirstDayOfMonth().ToString("d") + " - " + today.LastDayOfMonth().ToString("d")
                    }));
                return result;
            }
        }
    }
}
