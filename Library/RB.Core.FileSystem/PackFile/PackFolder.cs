using RB.Core.FileSystem.PackFile.Struct;

namespace RB.Core.FileSystem.PackFile;

public class PackFolder : IFolder
{
    public readonly PackEntry Entry;

    public PackFolder(string path, PackEntry entry, IFileSystem fileSystem)
    {
        Entry = entry;
        Path = path;
        FileSystem = fileSystem;
    }

    /// <inheritdoc />
    public IFileSystem FileSystem { get; }

    /// <inheritdoc />
    public string Name => Entry.Name;

    /// <inheritdoc />
    public IFolder Parent => FileSystem.GetFolder(PathUtil.GetFolderName(Path));

    /// <inheritdoc />
    public string Path { get; }

    /// <inheritdoc />
    public DateTime CreationTime => Entry.CreationTime;

    /// <inheritdoc />
    public DateTime ModifyTime => Entry.ModifyTime;

    /// <inheritdoc />
    public IFolder[] GetFolders()
    {
        return FileSystem.GetFolders(Path);
    }

    /// <inheritdoc />
    public IEnumerable<IFile> GetFiles()
    {
        return FileSystem.GetFiles(Path);
    }

    /// <inheritdoc />
    public string[] GetChildren()
    {
        return FileSystem.GetChildren(Path);
    }

    public override string ToString()
    {
        return Path;
    }
}