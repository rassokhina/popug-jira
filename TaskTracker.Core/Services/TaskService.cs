﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using TaskTracker.Core.Data;
using TaskTracker.Core.Dto;

namespace TaskTracker.Core.Services
{
    public sealed class TaskService: ITaskService
    {
        private readonly DefaultContext defaultContext;

        public TaskService(DefaultContext defaultContext)
        {
            this.defaultContext = defaultContext ?? throw new ArgumentNullException(nameof(defaultContext));
        }

        public async Task Reassign()
        {
            var tasks = await defaultContext.Tasks
                .Where(x => x.Status == Entities.TaskStatus.Open)
                .ToListAsync()
                .ConfigureAwait(false);
            var popugIds = await defaultContext.Popugs.Select(x => x.Id).ToArrayAsync().ConfigureAwait(false);

            if (popugIds.Length > 0)
            {
                Random rand = new Random();
                tasks.ForEach(t => {
                    int position = rand.Next(popugIds.Length);
                    t.PopugId = popugIds[position];
                });
                await defaultContext.SaveChangesAsync();
            }
        }

        public async Task Create(TaskCreateDto createDto)
        {
            var task = new Entities.Task
            {
                Description = createDto.Description,
                Created = DateTimeOffset.UtcNow,
                Status = Entities.TaskStatus.Open,
                Price = new Random().Next(10, 20)
            };
            await defaultContext.AddAsync(task);
            await defaultContext.SaveChangesAsync();
        }

        public async Task Finish(Guid taskId, Guid popugId)
        {
            var task = await defaultContext.Tasks.FindAsync(taskId).ConfigureAwait(false);
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            task.Status = Entities.TaskStatus.Finished;
            task.Finished = DateTimeOffset.UtcNow;
            await defaultContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<TaskDto>> GetList(Guid popugId)
        {
            return await defaultContext.Tasks
                .AsNoTracking()
                .Where(t => t.PopugId == popugId)
                .OrderBy(x => x.Created)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Description = t.Description,
                    Status = t.Status,
                    PopugId = t.PopugId,
                    Price = t.Price
                })
                .ToListAsync();
        }

    }
}