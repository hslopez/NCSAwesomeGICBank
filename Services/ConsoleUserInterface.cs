using AwesomeGICBank1.Interfaces;

namespace AwesomeGICBank1.Services
{
    public class ConsoleUserInterface : IUserInterface
    {
        public void WriteLine(string message) => Console.WriteLine(message);
        public void Write(string message) => Console.Write(message);
        public string? ReadLine() => Console.ReadLine();
        public void Clear() => Console.Clear();
    }
}
