using System;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using Service.BonusClientContext.Domain.Models;
using Service.BonusClientContext.Domain.Models.Events;
using Service.BonusClientContext.Postgres;
using Service.ClientProfile.Grpc;
using Service.ClientProfile.Grpc.Models.Requests;
using Service.PersonalData.Grpc;
using Service.PersonalData.Grpc.Contracts;
using Service.Registration.Domain.Models;

namespace Service.BonusClientContext.Jobs
{
    public class RegistrationUpdateJob
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<ContextUpdate> _publisher;
        private readonly ILogger<RegistrationUpdateJob> _logger;
        private readonly IPersonalDataServiceGrpc _personalData;
        private readonly IClientProfileService _clientProfile;
        public RegistrationUpdateJob(ISubscriber<ClientRegisterMessage> subscriber, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, IServiceBusPublisher<ContextUpdate> publisher, ILogger<RegistrationUpdateJob> logger, IPersonalDataServiceGrpc personalData, IClientProfileService clientProfile)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisher = publisher;
            _logger = logger;
            _personalData = personalData;
            _clientProfile = clientProfile;
            subscriber.Subscribe(HandleEvent);
            
        }

        private async ValueTask HandleEvent(ClientRegisterMessage registrationMessage)
        {
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var context = await ctx.ClientContexts.FirstOrDefaultAsync(t => t.ClientId == registrationMessage.TraderId);
                if (context == null)
                {
                    var pd = await _personalData.GetByIdAsync(
                        new GetByIdRequest() { Id = registrationMessage.TraderId });

                    var country = pd.PersonalData.CountryOfRegistration;

                    var profile = await _clientProfile.GetOrCreateProfile(new GetClientProfileRequest()
                    {
                        ClientId = registrationMessage.TraderId
                    });
                    
                    context = new ClientContext
                    {
                        ClientId = registrationMessage.TraderId,
                        Country = country,
                        RegistrationDate = DateTime.UtcNow,
                        ReferrerClientId = profile.ReferrerClientId,
                        HasReferrer = !string.IsNullOrWhiteSpace(profile.ReferrerClientId)
                    };
                    await ctx.UpsertAsync(new[] { context });

                    var update = new ContextUpdate
                    {
                        EventType = EventType.ClientRegistered,
                        ClientId = registrationMessage.TraderId,
                        Context = context,
                        RegistrationEvent = new RegistrationEvent()
                        {
                            RegistrationDate = DateTime.UtcNow
                        }
                    };

                    await _publisher.PublishAsync(update);
                    _logger.LogDebug("Sending Event with type {type} to client {clientId}", EventType.ClientRegistered.ToString(),registrationMessage.TraderId);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When handling registration update for clientId {clientId}", registrationMessage.TraderId);
                throw;
            }
        }
    }
}