namespace RB.Game.Client.ResourceLoader;

public class LoaderException<TExpectedType> : Exception
{
    public LoaderException(string path, TExpectedType expectedType)
    {
        Path = path;
        ExpectedType = expectedType;
    }

    public TExpectedType ExpectedType { get; }
    public string Path { get; }
    public override string Message => $"Can not load {Path} as [{nameof(TExpectedType)}]";
}