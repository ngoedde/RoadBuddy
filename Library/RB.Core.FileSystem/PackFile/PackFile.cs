using RB.Core.FileSystem.IO;
using RB.Core.FileSystem.PackFile.Struct;

namespace RB.Core.FileSystem.PackFile;

public class PackFile : IFile
{
    public DateTime CreateTime => Entry.CreationTime;
    public DateTime ModifyTime => Entry.ModifyTime;

    public IFileSystem FileSystem { get; }
    public string Path { get; }
    public string Name => Entry.Name;
    public string Extension => System.IO.Path.GetExtension(Name);
    
    public long Size => Entry.Size;

    public IFolder Parent => FileSystem.GetFolder(System.IO.Path.GetDirectoryName(Path) ?? "");
    
    public readonly PackEntry Entry;
    
    public PackFile(string path, PackEntry entry, IFileSystem fileSystem)
    {
        Path = path;
        FileSystem = fileSystem;
        
        Entry = entry;
    }

    

    public IFileReader OpenRead()
    {
        return FileSystem.OpenRead(Path);
    }
    

    public override string ToString()
    {
        return Path;
    }
}