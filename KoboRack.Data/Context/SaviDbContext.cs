using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KoboRack.Model.Entities;

namespace KoboRack.Data.Context
{
    public class SaviDbContext : IdentityDbContext<AppUser>
    {
        public SaviDbContext(DbContextOptions<SaviDbContext> options) : base(options)
        {
        }

		//public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AppUserTransaction> AppUserTransactions { get; set; }
        public DbSet<CardDetail> CardDetails { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupTransaction> GroupsTransaction { get; set; }
        public DbSet<Kyc> Kycs { get; set; }
        //public DbSet<Otp> Otps { get; set; }
        public DbSet<Saving> Savings { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletFunding> WalletFundings { get; set; }
        public DbSet<GroupSavingsMembers> GroupSavingsMembers { get; set; }
        public DbSet<FundingAnalytics> FundingAnalytics { get; set; }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var item in ChangeTracker.Entries<BaseEntity>())
            {
                switch (item.State)
                {
                    case EntityState.Modified:
                        item.Entity.ModifiedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Deleted:
                        item.Entity.IsDeleted = true;
                        break;
                    case EntityState.Added:
                        item.Entity.Id = Guid.NewGuid().ToString();
                        item.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    default:
                        break;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
