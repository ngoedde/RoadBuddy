namespace RB.Core;

public interface IRoadBuddyApp : IDisposable
{
    void Run();

    void Close();
}