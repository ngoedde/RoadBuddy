namespace RB.Game.Client.ResourceLoader;

public interface ILoadResult
{
    bool Success { get; }

    string Path { get; }

    string? Message { get; }
}