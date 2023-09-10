using RB.Core.FileSystem.IO;

namespace RB.Game.Client.ResourceLoader;

public abstract class ResourceLoader<TResult, TExpectedVal> : ILoader<TResult, TExpectedVal> where TResult : LoaderResult<TExpectedVal>
{
    private readonly IClientFileSystem _clientFileSystem;
    
    protected ResourceLoader(IClientFileSystem clientFileSystem)
    {
        _clientFileSystem = clientFileSystem;
    }

    public delegate void LoadEventHandler(TResult result);
    public event LoadEventHandler? Loaded;

    public delegate void LoadingEventHandler(string path);
    public event LoadingEventHandler? Loading;

    public virtual bool TryLoad(string path, out TResult result)
    {
        throw new NotImplementedException();
    }

    protected void OnLoaded(TResult result)
    {
        Loaded?.Invoke(result);
    }

    protected void OnLoading(string path)
    {
        Loading?.Invoke(path);
    }

    protected virtual IFileReader ReadFileFromMedia(string path)
    {
        //ToDo: A little hack to support lowercase/uppercase files. 
        if (!_clientFileSystem.Media.FileExists(path))
            path = path.ToLower();
        
        return _clientFileSystem.Media.OpenRead(path);
    }
    // protected virtual IFileReader ReadFileFromData(string path) => _clientFileSystem.Data.OpenRead(path);
}