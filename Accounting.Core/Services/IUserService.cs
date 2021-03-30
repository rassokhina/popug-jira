using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Shared.Events;

namespace Accounting.Core.Services
{
    public interface IUserService
    {
        Task Create(UserCreatedEvent createdEvent);
    }
}
