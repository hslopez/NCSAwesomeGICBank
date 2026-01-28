using AwesomeGICBank1.Interfaces;

namespace AwesomeGICBank1.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IPersistenceService _persistence;
        private decimal _balance;
        private readonly List<Transaction> _transactions = new();

        public BankAccountService(IPersistenceService persistence)
        {
            _persistence = persistence ?? throw new ArgumentNullException(nameof(persistence));
            // Load initial state from DB on construction
            LoadFromPersistenceAsync().GetAwaiter().GetResult();
        }

        private async Task LoadFromPersistenceAsync()
        {
            var (txs, bal) = await _persistence.LoadAsync();
            _transactions.Clear();
            _transactions.AddRange(txs);
            _balance = bal;
        }

        public decimal Balance => _balance;

        public void Deposit(decimal amount, DateTime? transactionTime = null)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive.", nameof(amount));

            _balance += amount;
            var time = transactionTime ?? DateTime.Now;
            _transactions.Add(new Transaction(time, amount, _balance));

            // Save after mutation (fire-and-forget or make method async)
            _ = SaveChangesAsync();
        }

        public void Withdraw(decimal amount, DateTime? transactionTime = null)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive.", nameof(amount));
            if (amount > _balance) throw new InvalidOperationException("Insufficient funds.");

            _balance -= amount;
            var time = transactionTime ?? DateTime.Now;
            _transactions.Add(new Transaction(time, -amount, _balance));

            _ = SaveChangesAsync();
        }

        private async Task SaveChangesAsync()
        {
            try
            {
                await _persistence.SaveAsync(_transactions.AsReadOnly(), _balance);
            }
            catch (Exception ex)
            {
                // Log error – don't throw to UI thread for now
                Console.Error.WriteLine($"Persistence error: {ex.Message}");
            }
        }

        public IReadOnlyList<Transaction> GetTransactionHistory() => _transactions.AsReadOnly();
    }
}
