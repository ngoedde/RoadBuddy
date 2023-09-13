using RB.Core.FileSystem;

namespace RB.Game.Client;

public interface IClientFileSystem
{
    public IFileSystem Data { get; }

    public IFileSystem Media { get; }
}