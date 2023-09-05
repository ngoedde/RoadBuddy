namespace RB.Game.Client.ResourceLoader;

public interface ILoaderResult<out TExpected>
{
    public bool Success { get; }
    
    public string Path { get; }
    
    public TExpected? Value { get; }
}