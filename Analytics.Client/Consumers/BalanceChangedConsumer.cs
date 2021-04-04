using System;
using System.Threading;
using System.Threading.Tasks;

using Analytics.Core.Commands;

using MassTransit;

using MediatR;

using Shared.Events;

namespace Analytics.Client.Consumers
{
    public class BalanceChangedConsumer : IConsumer<BalanceChangedEvent>
    {
        private readonly IMediator mediator;

        public BalanceChangedConsumer(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Consume(ConsumeContext<BalanceChangedEvent> context)
        {
            await mediator.Send(new ChangeBalanceCommand()
            {
                UserId = context.Message.UserId,
                WalletId = context.Message.WalletId,
                Time = context.Message.Time,
                BalanceChange = context.Message.BalanceChange,
            });
            Console.WriteLine($"Receive event {context.Message.EventName} in {context.Message.EventProducer}");
        }
    }
}
