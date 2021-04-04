using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Shared.Events;

namespace Analytics.Client.Services
{
    public interface ITaskService
    {
        Task Create(TaskCreatedEvent createdEvent);
    }
}
