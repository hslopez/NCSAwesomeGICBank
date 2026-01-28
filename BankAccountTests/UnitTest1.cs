using AwesomeGICBank1.Interfaces;
using AwesomeGICBank1.Services;
using FluentAssertions;

namespace BankAccountTests
{
    public class TestUserInterface : IUserInterface
    {
        private readonly StringWriter _writer;

        public TestUserInterface(StringWriter writer)
        {
            _writer = writer;
        }

        public void WriteLine(string message) => _writer.WriteLine(message);
        public void Write(string message) => _writer.Write(message);
        public string? ReadLine() => throw new NotImplementedException("Not needed for output tests");
        public void Clear() { }
    }

    public class FakePersistenceService : IPersistenceService
    {
        public Task SaveAsync(IReadOnlyList<Transaction> transactions, decimal currentBalance)
        {
            return Task.CompletedTask; // do nothing
        }

        public Task<(IReadOnlyList<Transaction> Transactions, decimal Balance)> LoadAsync()
        {
            // Return empty history and zero balance for most tests
            return Task.FromResult<(IReadOnlyList<Transaction>, decimal)>(
                (new List<Transaction>().AsReadOnly(), 0m));
        }
    }

    public class BankAccountTests
    {
        [Fact]
        public void New_account_should_have_zero_balance()
        {
            var persistence = new FakePersistenceService();
            var account = new BankAccountService(persistence);

            account.Balance.Should().Be(0m);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-50.75)]
        public void Deposit_with_non_positive_amount_should_throw(decimal invalidAmount)
        {
            var persistence = new FakePersistenceService();
            var account = new BankAccountService(persistence);

            Action act = () => account.Deposit(invalidAmount);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*positive*");
        }

        [Fact]
        public void Deposit_should_increase_balance_and_record_transaction()
        {
            var persistence = new FakePersistenceService();
            var account = new BankAccountService(persistence);
            decimal amount = 1000.50m;

            account.Deposit(amount);

            account.Balance.Should().Be(amount);

            var history = account.GetTransactionHistory();
            history.Should().HaveCount(1);

            var tx = history[0];
            tx.Amount.Should().Be(amount);
            tx.BalanceAfter.Should().Be(amount);
            tx.Date.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Withdraw_with_insufficient_funds_should_throw()
        {
            var persistence = new FakePersistenceService();
            var account = new BankAccountService(persistence);
            account.Deposit(300m);

            Action act = () => account.Withdraw(400m);

            act.Should().Throw<InvalidOperationException>()
               .WithMessage("*Insufficient funds*");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-50)]
        public void Withdraw_with_non_positive_amount_should_throw(decimal invalidAmount)
        {
            var persistence = new FakePersistenceService();
            var account = new BankAccountService(persistence);
            account.Deposit(500m);

            Action act = () => account.Withdraw(invalidAmount);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*positive*");
        }

        [Fact]
        public void Multiple_operations_should_update_balance_and_history_correctly()
        {
            var persistence = new FakePersistenceService();
            var account = new BankAccountService(persistence);

            account.Deposit(1000m);
            account.Withdraw(350.25m);
            account.Deposit(200m);
            account.Withdraw(50m);

            // Option 1 – cleanest
            account.Balance.Should().Be(1000m - 350.25m + 200m - 50m);

            var history = account.GetTransactionHistory();
            history.Should().HaveCount(4);

            history[0].Amount.Should().Be(1000m);
            history[0].BalanceAfter.Should().Be(1000m);

            history[1].Amount.Should().Be(-350.25m);
            history[1].BalanceAfter.Should().Be(649.75m);     // 1000 - 350.25

            history[2].Amount.Should().Be(200m);
            history[2].BalanceAfter.Should().Be(849.75m);     // 649.75 + 200

            history[3].Amount.Should().Be(-50m);
            history[3].BalanceAfter.Should().Be(799.75m);     // 849.75 - 50
        }

        [Fact]
        public void Transaction_amount_should_be_positive_for_deposit_and_negative_for_withdraw()
        {
            var persistence = new FakePersistenceService();
            var account = new BankAccountService(persistence);

            account.Deposit(500m);
            account.Withdraw(200m);

            var history = account.GetTransactionHistory();

            history[0].Amount.Should().BePositive();
            history[1].Amount.Should().BeNegative();
        }

        [Fact]
        public void PrintStatement_should_format_transactions_correctly()
        {
            var persistence = new FakePersistenceService();
            var account = new BankAccountService(persistence);

            // Use fixed times for deterministic output
            account.Deposit(500m, new DateTime(2022, 7, 8, 11, 12, 30));
            account.Withdraw(100m, new DateTime(2022, 7, 8, 11, 14, 15));

            // Capture output via StringWriter instead of real Console
            var stringWriter = new StringWriter();
            var ui = new TestUserInterface(stringWriter);           // ← helper below
            var printer = new ConsoleStatementPrinter(ui);

            printer.Print(account.GetTransactionHistory());

            var output = stringWriter.ToString().Replace("\r\n", "\n").Trim();
                        
            output.Should().Contain("8 Jul 2022 11:12:30 AM |  500.00 |   500.00");
            output.Should().Contain("8 Jul 2022 11:14:15 AM | -100.00 |   400.00");
        }
    }
}
