using System;
using System.Threading.Tasks;
using Shared.Events;
using MassTransit;
using Accounting.Core.Services;

namespace Accounting.Client.Consumers
{
    public class TaskCreatedConsumer : IConsumer<TaskCreatedEvent>
    {
        private readonly ITaskService taskService;

        public TaskCreatedConsumer(ITaskService taskService)
        {
            this.taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
        }

        public async Task Consume(ConsumeContext<TaskCreatedEvent> context)
        {
            await taskService.Create(context.Message);
        }
    }

}
