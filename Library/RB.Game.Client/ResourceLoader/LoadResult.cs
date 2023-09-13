namespace RB.Game.Client.ResourceLoader;

public abstract class LoadResult<TExpected> : ILoadResult
{
    protected LoadResult(bool success, string path, TExpected? value, string? message = null)
    {
        Success = success;
        Path = path;
        Value = value;
        Message = message;
    }

    public TExpected? Value { get; }
    public bool Success { get; }
    public string Path { get; }


    public string? Message { get; }

    public override string ToString()
    {
        return Path;
    }
}