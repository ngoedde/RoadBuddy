namespace RB.Core.FileSystem.Local;

public class LocalFolder : IFolder
{
    private readonly DirectoryInfo _directoryInfo;
    public LocalFolder(string path, IFileSystem fileSystem)
    {
        Path = path;
        FileSystem = fileSystem;
        
        _directoryInfo = new DirectoryInfo(path);
    }

    public IFileSystem FileSystem { get; }
    public string Path { get; }
    public string Name => System.IO.Path.GetFileName(Path);
    public IFolder Parent => FileSystem.GetFolder(System.IO.Path.GetDirectoryName(Path) ?? string.Empty);

    public DateTime CreationTime => _directoryInfo.CreationTime;
    
    public DateTime ModifyTime => _directoryInfo.LastWriteTime;
    
    public IFolder[] GetFolders()
    {
        return FileSystem.GetFolders(Path);
    }

    public IEnumerable<IFile> GetFiles()
    {
        return FileSystem.GetFiles(Path);
    }

    public string[] GetChildren()
    {
        return FileSystem.GetChildren(Path);
    }

    public void Delete()
    {
        FileSystem.Delete(Path);
    }

    public void Move(string destinationPath)
    {
        FileSystem.Move(Path, destinationPath);
    }

    public IFolder Copy(string destinationPath, bool recursive = true)
    {
        return FileSystem.CopyFolder(Path, destinationPath, recursive);
    }
}