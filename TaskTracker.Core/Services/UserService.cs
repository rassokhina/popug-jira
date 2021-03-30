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
    }
}
