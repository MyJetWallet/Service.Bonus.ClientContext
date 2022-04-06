using System;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Authorization.ServiceBus;
using MyJetWallet.Sdk.ServiceBus;
using Service.BonusClientContext.Domain.Models;
using Service.BonusClientContext.Postgres;
using Service.PersonalData.Grpc;
using Service.PersonalData.Grpc.Contracts;

namespace Service.BonusClientContext.Jobs
{
    public class LoginUpdateJob
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<ContextUpdate> _publisher;
        private readonly ILogger<LoginUpdateJob> _logger;
        private readonly IPersonalDataServiceGrpc _personalData;

        public LoginUpdateJob(ISubscriber<SessionAuditEvent> subscriber,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IServiceBusPublisher<ContextUpdate> publisher, ILogger<LoginUpdateJob> logger,
            IPersonalDataServiceGrpc personalData)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisher = publisher;
            _logger = logger;
            _personalData = personalData;
            subscriber.Subscribe(HandleEvent);
        }

        private async ValueTask HandleEvent(SessionAuditEvent session)
        {
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var context = await ctx.ClientContexts.FirstOrDefaultAsync(t => t.ClientId == session.Session.TraderId);
                if (context == null)
                {
                    var pd = await _personalData.GetByIdAsync(
                        new GetByIdRequest {Id = session.Session.TraderId});

                    var country = pd.PersonalData.CountryOfRegistration;
                    context = new ClientContext
                    {
                        ClientId = session.Session.TraderId,
                        Country = country,
                        RegistrationDate = DateTime.UtcNow
                    };
                    await ctx.UpsertAsync(new[] {context});
                }

                var update = new ContextUpdate
                {
                    EventType = EventType.ClientLoggedIn,
                    ClientId = session.Session.TraderId,
                    Context = context
                };

                await _publisher.PublishAsync(update);
                _logger.LogDebug("Sending Event with type {type} to client {clientId}",
                    EventType.ClientLoggedIn.ToString(), session.Session.TraderId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When handling registration update for clientId {clientId}",
                    session.Session.TraderId);
                throw;
            }
        }
    }
}