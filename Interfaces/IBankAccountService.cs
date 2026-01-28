using AwesomeGICBank1.Services;

namespace AwesomeGICBank1.Interfaces
{ 
// IBankAccountService.cs
    public interface IBankAccountService
    {
        decimal Balance { get; }
        void Deposit(decimal amount, DateTime? transactionTime = null);
        void Withdraw(decimal amount, DateTime? transactionTime = null);
        IReadOnlyList<Transaction> GetTransactionHistory();
    }
}