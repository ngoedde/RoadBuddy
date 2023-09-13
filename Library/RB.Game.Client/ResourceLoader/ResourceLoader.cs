using RB.Core.FileSystem.IO;

namespace RB.Game.Client.ResourceLoader;

public abstract class ResourceLoader<TLoadResult> : ILoader<TLoadResult> where TLoadResult : ILoadResult
{
    public delegate void LoadEventHandler(TLoadResult result);

    public delegate void LoadingEventHandler(string path);

    private readonly IClientFileSystem _clientFileSystem;

    protected ResourceLoader(IClientFileSystem clientFileSystem)
    {
        _clientFileSystem = clientFileSystem;
    }

    public abstract bool TryLoad(string path, out TLoadResult result);

    public event LoadEventHandler? Loaded;

    public event LoadingEventHandler? Loading;

    protected void OnLoaded(TLoadResult result)
    {
        Loaded?.Invoke(result);
    }

    protected void OnLoading(string path)
    {
        Loading?.Invoke(path);
    }

    protected virtual IFileReader GetFileFromMedia(string path)
    {
        //ToDo: A little hack to support lowercase/uppercase files. 
        if (!_clientFileSystem.Media.FileExists(path))
            path = path.ToLower();

        return _clientFileSystem.Media.OpenRead(path);
    }

    // protected virtual IFileReader ReadFileFromData(string path) => _clientFileSystem.Data.OpenRead(path);
}