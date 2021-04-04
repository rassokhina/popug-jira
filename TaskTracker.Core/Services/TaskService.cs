using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Core.Data;
using TaskTracker.Core.Dto;
using Shared.Events;

namespace TaskTracker.Core.Services
{
    public sealed class TaskService: ITaskService
    {
        private readonly DefaultContext defaultContext;
        private readonly IPublishEndpoint publishEndpoint;

        public TaskService(DefaultContext defaultContext, IPublishEndpoint publishEndpoint)
        {
            this.defaultContext = defaultContext ?? throw new ArgumentNullException(nameof(defaultContext));
            this.publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task Reassign()
        {
            var tasks = await defaultContext.Tasks
                .Where(x => x.Status == Entities.TaskStatus.Open)
                .ToListAsync()
                .ConfigureAwait(false);
            var userIds = await defaultContext.Users.Select(x => x.Id).ToArrayAsync().ConfigureAwait(false);

            if (userIds.Length > 0)
            {
                Random rand = new Random();
                tasks.ForEach(t => {
                    int position = rand.Next(userIds.Length);
                    t.UserId = userIds[position];
                });
                await defaultContext.SaveChangesAsync();

                /* TODO:
                 здесь нужно отправить событие TaskAssined, его будет
                 консьюмить сам сервис TaskTracker - и сообщение об ассайне задачи будет отправляться юзеру, а также Accounting и обновлять Task, 
                 Wallet (добавлять транзакцию с Debit = -Task.Price) и пересчитывать баланс
                */
            }
        }

        public async Task Create(TaskCreateDto createDto)
        {
            var task = new Entities.Task
            {
                Id = Guid.NewGuid(),
                Description = createDto.Description,
                Created = DateTimeOffset.UtcNow,
                Status = Entities.TaskStatus.Open,
            };
            await defaultContext.AddAsync(task);
            await defaultContext.SaveChangesAsync();
            
            await publishEndpoint.Publish(new TaskCreatedEvent
            {
                Id = task.Id,
                Description = task.Description,
                Created = task.Created,
                Status = (Shared.Events.TaskStatus)task.Status
            });
        }

        public async Task Finish(Guid taskId, string userId)
        {
            var task = await defaultContext.Tasks.FindAsync(taskId).ConfigureAwait(false);
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            task.Status = Entities.TaskStatus.Finished;
            task.Finished = DateTimeOffset.UtcNow;
            await defaultContext.SaveChangesAsync();

            /* TODO:
               здесь нужно отправить событие TaskFinished, его будет
               консьюмить Accounting и обновлять Task, Wallet (добавлять транзакцию с Credit = +Task.Price) и пересчитывать баланс,
               также будет консьюмить сервис  Analytics и обновлять статус Task и время завершения
              */
        }

        public async Task<IEnumerable<TaskDto>> GetList(string userId)
        {
            return await defaultContext.Tasks
                .AsNoTracking()
                .Where(t => t.UserId == userId)
                .OrderBy(x => x.Created)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Description = t.Description,
                    Status = t.Status,
                    UserId = t.UserId,
                })
                .ToListAsync();
        }

    }
}
