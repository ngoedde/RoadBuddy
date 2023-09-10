namespace RB.Game.Client.ResourceLoader;

public abstract class LoaderResult<TExpected>
{
    public bool Success { get; }
    
    public string Path { get; }
    
    public TExpected? Value { get; }
    
    public string? Message { get; }
    
    protected LoaderResult(bool success, string path, TExpected? value, string? message = null)
    {
        Success = success;
        Path = path;
        Value = value;
        Message = message;
    }

    public override string ToString()
    {
        return Path;
    }
}