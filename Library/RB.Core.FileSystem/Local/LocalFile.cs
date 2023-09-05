using RB.Core.FileSystem.IO;

namespace RB.Core.FileSystem.Local;

public class LocalFile : IFile
{
    private readonly FileInfo _fileInfo;

    public LocalFile(string filePath, IFileSystem fileSystem)
    {
        Path = filePath;
        FileSystem = fileSystem;

        _fileInfo = new FileInfo(filePath);
    }

    public IFileSystem FileSystem { get; }
    public IFolder Parent => FileSystem.GetFolder(System.IO.Path.GetDirectoryName(Path) ?? "");
    public string Path { get; }
    public string Name => _fileInfo.Name;
    public string Extension => _fileInfo.Extension;
    public long Size => _fileInfo.Length;

    public DateTime CreateTime => _fileInfo.CreationTime;
    
    public DateTime ModifyTime => _fileInfo.LastWriteTime;
    
    public void Move(string destinationPath)
    {
        FileSystem.Move(Path, destinationPath);
    }

    public IFile Copy(string destinationPath)
    {
        return FileSystem.CopyFile(Path, destinationPath);
    }

    public void Delete()
    {
        FileSystem.Delete(Path);
    }

    public IFileReader OpenRead()
    {
        return FileSystem.OpenRead(Path);
    }

    public IFileWriter OpenWrite()
    {
        return FileSystem.OpenWrite(Path);
    }
}