using System;
using System.Threading.Tasks;

using Analytics.Client.Services;

using Shared.Events;
using MassTransit;

namespace Analytics.Client.Consumers
{
    public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly IUserService userService;

        public UserCreatedConsumer(IUserService userService)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            await userService.Create(context.Message);
        }
    }
}
