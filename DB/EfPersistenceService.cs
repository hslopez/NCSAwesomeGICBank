using Microsoft.EntityFrameworkCore;
using AwesomeGICBank1.Services;
using AwesomeGICBank1.Interfaces;

namespace AwesomeGICBank1.DB
{
    public class EfPersistenceService : IPersistenceService
    {
        private readonly IDbContextFactory<BankDbContext> _contextFactory;

        public EfPersistenceService(IDbContextFactory<BankDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task SaveAsync(IReadOnlyList<Transaction> transactions, decimal currentBalance)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            // This line is probably the problem
            var existing = await context.Transactions.ToListAsync();
            context.Transactions.RemoveRange(existing);

            // Only adding the current list (which might be incomplete)
            context.Transactions.AddRange(transactions);
            await context.SaveChangesAsync();
        }

        public async Task<(IReadOnlyList<Transaction>, decimal)> LoadAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            //Console.WriteLine("Using database: " + context.Database.GetDbConnection().ConnectionString);

            var txs = await context.Transactions
                .OrderBy(t => t.Date)
                .ToListAsync();

            var balance = txs.Any() ? txs.Last().BalanceAfter : 0m;

            return (txs.AsReadOnly(), balance);
        }
    }
}
