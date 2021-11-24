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
using Service.Liquidity.Converter.Domain.Models;

namespace Service.BonusClientContext.Jobs
{
    public class TradeUpdateJob
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<ContextUpdate> _publisher;
        private readonly ILogger<TradeUpdateJob> _logger;

        public TradeUpdateJob(ISubscriber<SwapMessage> subscriber, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, IServiceBusPublisher<ContextUpdate> publisher, ILogger<TradeUpdateJob> logger)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisher = publisher;
            _logger = logger;
            subscriber.Subscribe(HandleEvent);
            
        }

        private async ValueTask HandleEvent(SwapMessage swap)
        {
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var context = await ctx.ClientContexts.FirstOrDefaultAsync(t => t.ClientId == swap.AccountId1) ??
                              new ClientContext
                              {
                                  ClientId = swap.AccountId1
                              };

                await ctx.UpsertAsync(new[] { context });

                var update = new ContextUpdate
                {
                    EventType = EventType.TradeMade,
                    ClientId = swap.AccountId1,
                    Context = context,
                    TradeEvent = new TradeEvent
                    {
                        TradeId = swap.Id,
                        FromAssetId = swap.AssetId1,
                        FromAmount = Decimal.Parse(swap.Volume1),
                        ToAssetId = swap.AssetId2,
                        ToAmount = Decimal.Parse(swap.Volume2),
                    },
                };

                await _publisher.PublishAsync(update);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When handling deposit update for clientId {clientId}", swap.AccountId1);
                throw;
            }
        }
    }
}