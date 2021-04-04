using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Analytics.Client.Data;
using Shared.Events;

namespace Analytics.Client.Services
{
    public sealed class UserService: IUserService
    {
        private readonly DefaultContext defaultContext;

        public UserService(DefaultContext defaultContext)
        {
            this.defaultContext = defaultContext ?? throw new ArgumentNullException(nameof(defaultContext));
        }

        public async Task Create(UserCreatedEvent createdEvent)
        {
            var user = new Entities.User()
            {
                Id = createdEvent.Id,
                Username = createdEvent.Username,
                Created = DateTimeOffset.UtcNow,
                Role = createdEvent.Role
            };
            await defaultContext.AddAsync(user);
            await defaultContext.SaveChangesAsync();
        }

        public async Task Finish(TaskFinishedEvent finishedEvent)
        {
            // TODO: обновить задачу, указать время завершения
        }

        public async Task ChangeWallet()
        {
            // TODO: обновить кошелек при получении события изменения баланса
        }
    }
}
