using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using Microsoft.EntityFrameworkCore;

using TaskTracker.Core.Data;
using TaskTracker.Core.Dto;

namespace TaskTracker.Core.Queries
{
    public class GetTasksQuery : IRequest<IEnumerable<TaskDto>>
    {
        public string UserId { get; set; }

        public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, IEnumerable<TaskDto>>
        {
            private readonly DefaultContext defaultContext;

            public GetTasksQueryHandler(DefaultContext defaultContext)
            {
                this.defaultContext = defaultContext ?? throw new ArgumentNullException(nameof(defaultContext));
            }

            public async Task<IEnumerable<TaskDto>> Handle(GetTasksQuery query,
                                                             CancellationToken cancellationToken)
            {
                return await defaultContext.Tasks
                    .AsNoTracking()
                    .Where(t => t.UserId == query.UserId)
                    .OrderBy(x => x.CreatedAt)
                    .Select(t => new TaskDto
                    {
                        Id = t.Id,
                        Description = t.Description,
                        Status = t.Status,
                        UserId = t.UserId,
                    })
                    .ToListAsync(cancellationToken);
            }
        }
    }
}
