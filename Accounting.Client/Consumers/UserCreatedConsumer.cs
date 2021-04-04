using System;
using System.Threading.Tasks;
using Shared.Events;
using MassTransit;
using Accounting.Core.Services;

namespace Accounting.Client.Consumers
{
    public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly IUserService userService;
        private readonly IWalletService walletService;


        public UserCreatedConsumer(IUserService userService, IWalletService walletService)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
        }

        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            // create user
            await userService.Create(context.Message);

            // create wallet
            await walletService.Create();
        }
    }
}
