using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Analytics.Client.Data;
using Analytics.Client.Entities;

using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using Task = System.Threading.Tasks.Task;

namespace Analytics.Client.Services
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

        public async Task Create(TaskCreatedEvent createdEvent)
        {
            var task = new Entities.Task
            {
                Id = createdEvent.Id,
                Description = createdEvent.Description,
                Created = DateTimeOffset.UtcNow,
                Status = (Entities.TaskStatus)createdEvent.Status
            };
            await defaultContext.AddAsync(task);
            await defaultContext.SaveChangesAsync();
        }
    }
}
