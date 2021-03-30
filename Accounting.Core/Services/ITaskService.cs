using Shared.Events;
using System.Threading.Tasks;

namespace Accounting.Core.Services
{
    public interface ITaskService
    {
        Task Create(TaskCreatedEvent createdEvent);
    }
}
