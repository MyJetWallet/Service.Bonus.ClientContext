using System;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using Service.BonusClientContext.Domain.Models;
using Service.BonusClientContext.Domain.Models.Events;
using Service.BonusClientContext.Postgres;
using Service.ClientProfile.Domain.Models;
using Service.KYC.Domain.Models.Enum;
using Service.KYC.Domain.Models.Messages;

namespace Service.BonusClientContext.Jobs
{
    public class KycUpdateJob
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<ContextUpdate> _publisher;
        private readonly ILogger<KycUpdateJob> _logger;

        public KycUpdateJob(ISubscriber<KycProfileUpdatedMessage> subscriber, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, IServiceBusPublisher<ContextUpdate> publisher, ILogger<KycUpdateJob> logger)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisher = publisher;
            _logger = logger;
            subscriber.Subscribe(HandleEvent);
            
        }

        private async ValueTask HandleEvent(KycProfileUpdatedMessage message)
        {
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var context = await ctx.ClientContexts.FirstOrDefaultAsync(t => t.ClientId == message.NewProfile.ClientId) ??
                              new ClientContext
                              {
                                  ClientId = message.NewProfile.ClientId
                              };

                if (context.KycDepositAllowed == false && (message.NewProfile.DepositStatus == KycOperationStatus.Allowed || message.NewProfile.DepositStatus == KycOperationStatus.AllowedWithKycAlert))
                {
                    context.KycDepositAllowed = true;
                    await ctx.UpsertAsync(new[] { context });
                    
                    var update = new ContextUpdate
                    {
                        EventType = EventType.ReferrerAdded,
                        ClientId = message.NewProfile.ClientId,
                        Context = context,
                        KycEvent = new KYCEvent()
                        {
                            KycDepositPassed = true
                        }
                    };
                    await _publisher.PublishAsync(update);
                }
                
                if (context.KycTradeAllowed == false && (message.NewProfile.TradeStatus == KycOperationStatus.Allowed || message.NewProfile.TradeStatus == KycOperationStatus.AllowedWithKycAlert))
                {
                    context.KycTradeAllowed = true;
                    await ctx.UpsertAsync(new[] { context });
                    
                    var update = new ContextUpdate
                    {
                        EventType = EventType.ReferrerAdded,
                        ClientId = message.NewProfile.ClientId,
                        Context = context,
                        KycEvent = new KYCEvent()
                        {
                            KycTradePassed = true
                        }
                    };
                    await _publisher.PublishAsync(update);
                }
                
                if (context.KycWithdrawalAllowed == false && (message.NewProfile.WithdrawalStatus == KycOperationStatus.Allowed || message.NewProfile.WithdrawalStatus == KycOperationStatus.AllowedWithKycAlert))
                {
                    context.KycWithdrawalAllowed = true;
                    await ctx.UpsertAsync(new[] { context });
                    
                    var update = new ContextUpdate
                    {
                        EventType = EventType.ReferrerAdded,
                        ClientId = message.NewProfile.ClientId,
                        Context = context,
                        KycEvent = new KYCEvent()
                        {
                            KycWithdrawalPassed = true
                        }
                    };
                    await _publisher.PublishAsync(update);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When handling KYC update for clientId {clientId}", message.NewProfile.ClientId);
                throw;
            }
        }
    }
}