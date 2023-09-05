using RB.Core.FileSystem.IO;

namespace RB.Game.Client.ResourceLoader;

public abstract class ResourceLoader<TResult, TExpectedVal> : ILoader<TResult, TExpectedVal> where TResult : LoaderResult<TExpectedVal>
{
    private readonly IClientFileSystem _clientFileSystem;
    
    protected ResourceLoader(IClientFileSystem clientFileSystem)
    {
        _clientFileSystem = clientFileSystem;
    }

    public delegate void LoadEvent(TResult result);
    public event LoadEvent? Loaded;

    public delegate void LoadingEvent(string path);
    public event LoadingEvent? Loading;

    public virtual bool TryLoad(string path, out TResult result)
    {
        throw new NotImplementedException();
    }

    protected virtual void OnLoaded(TResult result)
    {
        Loaded?.Invoke(result);
    }

    protected virtual void OnLoading(string path)
    {
        Loading?.Invoke(path);
    }

    protected virtual IFileReader ReadFileFromMedia(string path) => _clientFileSystem.Media.OpenRead(path);
    protected virtual IFileReader ReadFileFromData(string path) => _clientFileSystem.Data.OpenRead(path);
}