using System;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain.ServiceBus.Models;
using MyJetWallet.Sdk.ServiceBus;
using Service.Bitgo.DepositDetector.Domain.Models;
using Service.BonusClientContext.Domain.Models;
using Service.BonusClientContext.Domain.Models.Events;
using Service.BonusClientContext.Postgres;

namespace Service.BonusClientContext.Jobs
{
    public class RegistrationUpdateJob
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<ContextUpdate> _publisher;
        private readonly ILogger<RegistrationUpdateJob> _logger;

        public RegistrationUpdateJob(ISubscriber<ClientRegistrationMessage> subscriber, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, IServiceBusPublisher<ContextUpdate> publisher, ILogger<RegistrationUpdateJob> logger)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisher = publisher;
            _logger = logger;
            subscriber.Subscribe(HandleEvent);
            
        }

        private async ValueTask HandleEvent(ClientRegistrationMessage registrationMessage)
        {
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var context = await ctx.ClientContexts.FirstOrDefaultAsync(t => t.ClientId == registrationMessage.ClientId.ClientId);
                if (context == null)
                {
                    context = new ClientContext
                    {
                        ClientId = registrationMessage.ClientId.ClientId
                    };
                    await ctx.UpsertAsync(new[] { context });

                    var update = new ContextUpdate
                    {
                        EventType = EventType.ClientRegistered,
                        ClientId = registrationMessage.ClientId.ClientId,
                        Context = context,
                        RegistrationEvent = new RegistrationEvent()
                    };

                    await _publisher.PublishAsync(update);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When handling registration update for clientId {clientId}", registrationMessage.ClientId);
                throw;
            }
        }
    }
}