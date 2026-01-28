using AwesomeGICBank1.DB;
using AwesomeGICBank1.Interfaces;
using AwesomeGICBank1.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace AwesomeGICBank
{
    class Program
    {
        static void Main(string[] args)
        {

            var services = new ServiceCollection();

            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "awesome_gic_bank.db");
            services.AddDbContextFactory<BankDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}")
                       .ConfigureWarnings(w => w.Ignore(RelationalEventId.NonTransactionalMigrationOperationWarning)));
            services.AddSingleton<IUserInterface, ConsoleUserInterface>();
            services.AddSingleton<IPersistenceService, EfPersistenceService>();
            services.AddSingleton<IBankAccountService, BankAccountService>();
            services.AddSingleton<IStatementPrinter, ConsoleStatementPrinter>();

            var serviceProvider = services.BuildServiceProvider();

            var ui = serviceProvider.GetRequiredService<IUserInterface>();
            var account = serviceProvider.GetRequiredService<IBankAccountService>();
            var printer = serviceProvider.GetRequiredService<IStatementPrinter>();
                        

            ui.WriteLine("Welcome to AwesomeGIC Bank! What would you like to do?");
            PrintMenu(ui);

            while (true)
            {
                ui.Write("> ");
                string? input = ui.ReadLine()?.Trim().ToUpperInvariant();

                if (string.IsNullOrWhiteSpace(input))
                {
                    ui.WriteLine("Invalid input. Please choose an option.");
                    PrintMenu(ui);
                    continue;
                }

                char choice = input[0];

                switch (choice)
                {
                    case 'D':
                        HandleDeposit(ui, account);
                        break;

                    case 'W':
                        HandleWithdraw(ui, account);
                        break;

                    case 'P':
                        printer.Print(account.GetTransactionHistory());
                        PrintContinuePrompt(ui);
                        break;

                    case 'Q':
                        ui.WriteLine("Thank you for banking with AwesomeGIC Bank.");
                        ui.WriteLine("Have a nice day!");
                        return;

                    default:
                        ui.WriteLine("Invalid option. Please choose D, W, P or Q.");
                        PrintMenu(ui);
                        break;
                }
            }
        }

        private static void PrintMenu(IUserInterface ui)
        {
            ui.WriteLine("[D]eposit");
            ui.WriteLine("[W]ithdraw");
            ui.WriteLine("[P]rint statement");
            ui.WriteLine("[Q]uit");
        }

        private static void PrintContinuePrompt(IUserInterface ui)
        {
            ui.WriteLine("");
            ui.WriteLine("Is there anything else you'd like to do?");
            PrintMenu(ui);
        }

        private static void HandleDeposit(IUserInterface ui, IBankAccountService account)
        {
            ui.WriteLine("Please enter the amount to deposit:");
            string? input = ui.ReadLine()?.Trim();

            if (!decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount) || amount <= 0)
            {
                ui.WriteLine("Invalid amount. Please enter a positive number.");
                PrintContinuePrompt(ui);
                return;
            }

            try
            {
                account.Deposit(amount);
                ui.WriteLine($"Thank you. ${amount:F2} has been deposited to your account.");
            }
            catch (Exception ex)
            {
                ui.WriteLine($"Error: {ex.Message}");
            }

            PrintContinuePrompt(ui);
        }

        private static void HandleWithdraw(IUserInterface ui, IBankAccountService account)
        {
            ui.WriteLine("Please enter the amount to withdraw:");
            string? input = ui.ReadLine()?.Trim();

            if (!decimal.TryParse(input, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var amount) || amount <= 0)
            {
                ui.WriteLine("Invalid amount. Please enter a positive number.");
                PrintContinuePrompt(ui);
                return;
            }

            try
            {
                account.Withdraw(amount);
                ui.WriteLine($"Thank you. ${amount:F2} has been withdrawn.");
            }
            catch (InvalidOperationException)
            {
                ui.WriteLine("Insufficient funds.");
            }
            catch (Exception ex)
            {
                ui.WriteLine($"Error: {ex.Message}");
            }

            PrintContinuePrompt(ui);
        }
    }

}