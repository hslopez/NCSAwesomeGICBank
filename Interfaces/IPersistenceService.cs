using AwesomeGICBank1.Services;

namespace AwesomeGICBank1.Interfaces
{
    // IPersistenceService.cs
    public interface IPersistenceService
    {
        Task SaveAsync(IReadOnlyList<Transaction> transactions, decimal currentBalance);
        Task<(IReadOnlyList<Transaction> Transactions, decimal Balance)> LoadAsync();
    }
}
