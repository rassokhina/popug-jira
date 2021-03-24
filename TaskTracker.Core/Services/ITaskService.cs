using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TaskTracker.Core.Dto;

namespace TaskTracker.Core.Services
{
    public interface ITaskService
    {
        Task Reassign();

        Task<IEnumerable<TaskDto>> GetList(Guid userId);

        Task Create(TaskCreateDto createDto);

        Task Finish(Guid taskId, Guid userId);
    }
}
