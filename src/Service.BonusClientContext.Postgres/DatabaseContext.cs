using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Postgres;
using Service.BonusClientContext.Domain.Models;

namespace Service.BonusClientContext.Postgres
{
    public class DatabaseContext : MyDbContext
    {
        public const string Schema = "bonusprogram";

        public const string ContextsTableName = "clientcontexts";

        public DbSet<ClientContext> ClientContexts { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);
            modelBuilder.Entity<ClientContext>().ToTable(ContextsTableName);
            modelBuilder.Entity<ClientContext>().HasKey(e => e.ClientId);
            
            modelBuilder.Entity<ClientContext>().HasIndex(e => e.ClientId);
            modelBuilder.Entity<ClientContext>().Property(e => e.ClientId).HasMaxLength(128);
            modelBuilder.Entity<ClientContext>().Property(e => e.HasReferrals).HasDefaultValue(false);
            modelBuilder.Entity<ClientContext>().Property(e => e.HasReferrer).HasDefaultValue(false);
            modelBuilder.Entity<ClientContext>().Property(e => e.ReferrerClientId).HasMaxLength(128);
            modelBuilder.Entity<ClientContext>().Property(e => e.LastRecord); 
            modelBuilder.Entity<ClientContext>().Property(e => e.KycDepositAllowed).HasDefaultValue(false);
            modelBuilder.Entity<ClientContext>().Property(e => e.KycTradeAllowed).HasDefaultValue(false);
            modelBuilder.Entity<ClientContext>().Property(e => e.KycWithdrawalAllowed).HasDefaultValue(false);
            modelBuilder.Entity<ClientContext>().Property(e => e.LastRecord).HasDefaultValue(DateTime.MinValue); 

            
            modelBuilder.Entity<ClientContext>().HasIndex(e => e.ReferrerClientId);
            
            base.OnModelCreating(modelBuilder);
        }
        
        public async Task<int> UpsertAsync(IEnumerable<ClientContext> entities)
        {
            var result = await ClientContexts.UpsertRange(entities).AllowIdentityMatch().RunAsync();
            return result;
        }
        
    }
    

}
