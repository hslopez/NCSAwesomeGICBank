using Microsoft.EntityFrameworkCore;

namespace AwesomeGICBank1.Services
{
    public class Transaction
    {
        // Parameterless constructor – required for EF Core to create instances
        // Can be private/protected if you want to force factory usage
        private Transaction() { }   // ← add this (private is fine)

        // Your existing constructor
        public Transaction(DateTime date, decimal amount, decimal balanceAfter)
        {
            Date = date;
            Amount = amount;
            BalanceAfter = balanceAfter;
        }

        public int Id { get; private set; }
        public DateTime Date { get; private set; }         // make setters private if immutable
        public decimal Amount { get; private set; }
        public decimal BalanceAfter { get; private set; }

        // Optional: If you want to keep immutability strict, you can even remove public setters
        // But EF Core will still use the parameterless ctor + property setters internally
    }
}
