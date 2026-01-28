using Microsoft.EntityFrameworkCore;
using AwesomeGICBank1.Services;

namespace AwesomeGICBank1.DB
{
    public class BankDbContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; } = null!;

        public BankDbContext(DbContextOptions<BankDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.BalanceAfter)
                .HasPrecision(18, 2);
        }
    }
}
