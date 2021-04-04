using Accounting.Core.Data;
using MassTransit;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Core.Services
{
    public sealed class WalletService: IWalletService
    {
        private readonly DefaultContext defaultContext;
        private readonly IPublishEndpoint publishEndpoint;

        public WalletService(DefaultContext defaultContext, IPublishEndpoint publishEndpoint)
        {
            this.defaultContext = defaultContext ?? throw new ArgumentNullException(nameof(defaultContext));
            this.publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task Create()
        {
            // TODO: создать кошелек для пользователя
        
        }
    }
}
