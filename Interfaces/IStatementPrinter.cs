// IStatementPrinter.cs
using AwesomeGICBank1.Services;
namespace AwesomeGICBank1.Interfaces
{
    public interface IStatementPrinter
    {
        void Print(IReadOnlyList<Transaction> transactions);
    }
}