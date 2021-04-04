using Accounting.Core.Data;
using MassTransit;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Core.Services
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
            var task = new Accounting.Core.Entities.Task
            {
                Id = createdEvent.Id,
                Description = createdEvent.Description,
                Created = DateTimeOffset.UtcNow,
                Status = (Accounting.Core.Entities.TaskStatus)createdEvent.Status,
                Price = new Random().Next(10, 40)
            };
            await defaultContext.AddAsync(task);
            await defaultContext.SaveChangesAsync();
        }

        public async Task ChangeWallet(TaskAssignedEvent assignedEvent)
        {
            // TODO: здесь в транакции обновить Task и баланс кошелька, продьюсить событие BalanceChanged
        }

        public async Task ChangeWallet(TaskFinishedEvent finishedEvent)
        {
            // TODO: здесь в транакции обновить Task и баланс кошелька, продьюсить событие BalanceChanged
        }

        //TODO: отдельно написать джобу, которая раз в день обнуляет баланс и отправляет событие BalanceReset
    }
}
