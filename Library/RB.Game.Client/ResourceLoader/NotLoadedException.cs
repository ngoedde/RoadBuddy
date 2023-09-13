namespace RB.Game.Client.ResourceLoader;

public class NotLoadedException : Exception
{
    public NotLoadedException(string path)
    {
        Path = path;
    }

    public string Path { get; }

    public override string Message => $"Resource '{Path}' is not loaded!";
}