using System;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using Service.Bitgo.DepositDetector.Domain.Models;
using Service.BonusClientContext.Domain.Models;
using Service.BonusClientContext.Domain.Models.Events;
using Service.BonusClientContext.Postgres;

namespace Service.BonusClientContext.Jobs
{
    public class DepositUpdateJob
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<ContextUpdate> _publisher;
        private readonly ILogger<DepositUpdateJob> _logger;

        public DepositUpdateJob(ISubscriber<Deposit> subscriber, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, IServiceBusPublisher<ContextUpdate> publisher, ILogger<DepositUpdateJob> logger)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisher = publisher;
            _logger = logger;
            subscriber.Subscribe(HandleEvent);
            
        }

        private async ValueTask HandleEvent(Deposit deposit)
        {
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var context = await ctx.ClientContexts.FirstOrDefaultAsync(t => t.ClientId == deposit.ClientId) ??
                              new ClientContext
                              {
                                  ClientId = deposit.ClientId
                              };

                //TODO: deposit actions
                await ctx.UpsertAsync(new[] { context });

                var update = new ContextUpdate
                {
                    EventType = EventType.DepositMade,
                    ClientId = deposit.ClientId,
                    Context = context,
                    DepositEvent = new DepositEvent
                    {
                        DepositId = deposit.Id.ToString(),
                        AssetId = deposit.AssetSymbol,
                        Amount = (decimal)deposit.Amount,
                    },
                };

                await _publisher.PublishAsync(update);
                
                _logger.LogDebug("Sending Event with type {type} to client {clientId}", 
                    EventType.DepositMade.ToString(), deposit.ClientId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When handling deposit update for clientId {clientId}", deposit.ClientId);
                throw;
            }
        }
    }
}