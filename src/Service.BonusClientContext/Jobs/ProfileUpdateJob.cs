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

namespace Service.BonusClientContext.Jobs
{
    public class ProfileUpdateJob
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<ContextUpdate> _publisher;
        private readonly ILogger<ProfileUpdateJob> _logger;

        public ProfileUpdateJob(ISubscriber<ClientProfileUpdateMessage> subscriber, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, IServiceBusPublisher<ContextUpdate> publisher, ILogger<ProfileUpdateJob> logger)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisher = publisher;
            _logger = logger;
            subscriber.Subscribe(HandleEvent);
            
        }

        private async ValueTask HandleEvent(ClientProfileUpdateMessage message)
        {
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var context = await ctx.ClientContexts.FirstOrDefaultAsync(t => t.ClientId == message.NewProfile.ClientId) ??
                              new ClientContext
                              {
                                  ClientId = message.NewProfile.ClientId
                              };

                if (context.HasReferrer == false && !string.IsNullOrWhiteSpace(message.NewProfile.ReferrerClientId))
                {
                    context.HasReferrer = true;
                    context.ReferrerClientId = message.NewProfile.ReferrerClientId;
                    await ctx.UpsertAsync(new[] { context });
                    
                    var update = new ContextUpdate
                    {
                        EventType = EventType.ReferrerAdded,
                        ClientId = message.NewProfile.ClientId,
                        Context = context,
                        ReferrerAddedEvent = new ReferrerAddedEvent
                        {
                            ReferrerClientId = message.NewProfile.ReferrerClientId
                        },
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