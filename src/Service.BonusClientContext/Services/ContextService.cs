using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.BonusClientContext.Domain.Models;
using Service.BonusClientContext.Grpc;
using Service.BonusClientContext.Grpc.Models;
using Service.BonusClientContext.Postgres;
using Service.BonusClientContext.Settings;

namespace Service.BonusClientContext.Services
{
    public class ContextService: IContextService
    {
        private readonly ILogger<ContextService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public ContextService(ILogger<ContextService> logger, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task<ClientContext> GetContextByClientId(GetContextRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            return await ctx.ClientContexts.FirstOrDefaultAsync(t => t.ClientId == request.ClientId) ??
                          new ClientContext
                          {
                              ClientId = request.ClientId
                          };
        }

        public async Task<AllContextsResponse> GetAllContexts()
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var contexts = await ctx.ClientContexts.ToListAsync();
            return new AllContextsResponse()
            {
                Contexts = contexts
            };
        }
    }
}
