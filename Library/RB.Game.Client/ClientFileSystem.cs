using Microsoft.Extensions.Options;
using RB.Core.FileSystem;
using RB.Game.Client.Config;

namespace RB.Game.Client;

public class ClientFileSystem : IClientFileSystem
{
    public ClientFileSystem(IOptions<FileSystemConfig> config)
    {
        Media = new PackFileSystem(config.Value.Media.Path, config.Value.Media.Key);
        Data = new PackFileSystem(config.Value.Data.Path, config.Value.Data.Key);
    }

    public IFileSystem Data { get; }

    public IFileSystem Media { get; }
}