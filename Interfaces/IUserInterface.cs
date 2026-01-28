// IUserInterface.cs  (abstracts console interaction)
namespace AwesomeGICBank1.Interfaces
{
    public interface IUserInterface
    {
        void WriteLine(string message);
        void Write(string message);
        string? ReadLine();
        void Clear();                 // optional – can be useful in some UIs
    }
}