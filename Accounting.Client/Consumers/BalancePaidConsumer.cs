using System;
using System.Threading;
using System.Threading.Tasks;

using MassTransit;

using MediatR;

using Shared.Events;

namespace Accounting.Client.Consumers
{
    public class BalancePaidConsumer : IConsumer<BalancePaidEvent>
    {
        private readonly IMediator mediator;

        public BalancePaidConsumer(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Consume(ConsumeContext<BalancePaidEvent> context)
        {
            Console.WriteLine($"Receive event {context.Message.EventName} in {context.Message.EventProducer}");
            Console.WriteLine($"Send notification to user about balance paid {context.Message.Paid}");
        }
    }
}
