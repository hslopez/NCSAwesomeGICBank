// File: DB/BankDbContextFactory.cs  (or put it in the same folder/namespace as BankDbContext)

using AwesomeGICBank1.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AwesomeGICBank.DB  // ← adjust to match your actual namespace
{
    public class BankDbContextFactory : IDesignTimeDbContextFactory<BankDbContext>
    {
        public BankDbContext CreateDbContext(string[] args)
        {
            // Hard-code the connection string for design-time (migrations)
            // You can later make this more flexible (appsettings.json, env vars, etc.)
            var optionsBuilder = new DbContextOptionsBuilder<BankDbContext>();

            optionsBuilder.UseSqlite("Data Source=awesome_gic_bank.db");  // ← your SQLite connection string

            // Optional: enable sensitive data logging or detailed errors for debugging
            // optionsBuilder.EnableSensitiveDataLogging();
            // optionsBuilder.LogTo(Console.WriteLine);

            return new BankDbContext(optionsBuilder.Options);
        }
    }
}