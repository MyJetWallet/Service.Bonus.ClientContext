using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using Service.BonusClientContext.Domain.Models;
using Service.BonusClientContext.Postgres;

namespace Service.BonusClientContext.Services
{
    public class ManualUpdateService
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<ContextUpdate> _publisher;
        private readonly ILogger<ManualUpdateService> _logger;

        public ManualUpdateService(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, IServiceBusPublisher<ContextUpdate> publisher, ILogger<ManualUpdateService> logger)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisher = publisher;
            _logger = logger;
            
        }
        
        public async Task CheckAllClients()
        {
            _logger.LogInformation("Starting manual update for all clients");
            try
            {
                List<ClientContext> clientContexts;
                const int take = 500;
                var skip = 0;

                do
                {
                    await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                    clientContexts = await ctx.ClientContexts.OrderBy(t => t.ClientId).Skip(skip).Take(take)
                        .ToListAsync();

                    var updates = clientContexts.Select(context => new ContextUpdate()
                            {EventType = EventType.ManualCheckEvent, ClientId = context.ClientId, Context = context})
                        .ToList();
                    await _publisher.PublishAsync(updates);
                    
                    skip += take;
                    if(clientContexts.Any())
                        _logger.LogInformation("Updating clients from {clientIdFirst} to {clientIdEnd}. Batch count {count}", clientContexts.First().ClientId, clientContexts.Last().ClientId, clientContexts.Count);
                    else
                        _logger.LogInformation("Manual update finished");
                    
                    await Task.Delay(5000);
                } while (clientContexts.Any());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When updating all clients");
                throw;
            }
        }
        
    }
}