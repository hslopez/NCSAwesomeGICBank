using AwesomeGICBank1.Interfaces;
using AwesomeGICBank1.Services;

namespace AwesomeGICBank1.Services
{
    public class ConsoleStatementPrinter : IStatementPrinter
    {
        private readonly IUserInterface _ui;

        public ConsoleStatementPrinter(IUserInterface ui)
        {
            _ui = ui;
        }

        public void Print(IReadOnlyList<Transaction> transactions)
        {
            if (transactions.Count == 0)
            {
                _ui.WriteLine("No transactions yet.");
                return;
            }

            _ui.WriteLine("Date                  | Amount  | Balance");

            foreach (var tx in transactions)
            {
                string dateStr = tx.Date.ToString("d MMM yyyy h:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture)
                                       .Replace("  ", " "); // fix double space

                string amountStr = (tx.Amount >= 0 ? tx.Amount : -tx.Amount).ToString("F2").PadLeft(7);
                if (tx.Amount < 0) amountStr = "-" + amountStr.Trim();

                string balanceStr = tx.BalanceAfter.ToString("F2").PadLeft(8);

                _ui.WriteLine($"{dateStr,-21} | {amountStr} | {balanceStr}");
            }
        }
    }
}
