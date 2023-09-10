using System.Text;
using RB.Core.FileSystem.IO;
using RB.Core.FileSystem.PackFile;
using RB.Core.FileSystem.PackFile.Cryptography;
using RB.Core.FileSystem.PackFile.IO;
using RB.Core.FileSystem.PackFile.Struct;

namespace RB.Core.FileSystem;

public class PackFileSystem : IFileSystem, IDisposable
{
    #region Properties
    
    /// <inheritdoc />
    public IFolder Root => new PackFolder("", new PackEntry
    {
        CreationTime = DateTime.Now,
        ModifyTime = DateTime.Now,
        DataPosition = 256,
        Name = string.Empty,
        NextBlock = 0,
        Size = 0,
        Type = PackEntryType.Folder
    }, this);

    /// <inheritdoc />
    public string BasePath { get; }

    /// <inheritdoc />
    public char PathSeparator => '\\';
    
    public Encoding Encoding { get; set; } = Encoding.Default;

    #endregion

    #region Constructor
    
    private readonly FileStream _fileStream;
    private readonly PackArchive _archive;
    
    public PackFileSystem(string path)
    {
        BasePath = path;

        var packFileReader = new PackReader();

        _fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

        _archive = packFileReader.Read(_fileStream, null,  PathSeparator);
        
        PathUtil.PathSeparator = PathSeparator;
    }

    public PackFileSystem(string path, string password, byte[] salt)
    {
        BasePath = path;

        _fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

        var packFileReader = new PackReader();

        var key = BlowfishUtil.GenerateFinalBlowfishKey(password, salt);
        var blowfish = new Blowfish(key);

        _archive = packFileReader.Read(_fileStream, blowfish, PathSeparator);
        
        PathUtil.PathSeparator = PathSeparator;
    }

    public PackFileSystem(string path, string password) : this(path, password,
        new byte[] { 0x03, 0xF8, 0xE4, 0x44, 0x88, 0x99, 0x3F, 0x64, 0xFE, 0x35 })
    {
    }

    #endregion

    #region Implementations

    /// <inheritdoc />
    public bool FileExists(string path)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        if (!_archive.TryGetEntry(path, out var entry))
            return false;

        return entry is { Type: PackEntryType.File };
    }

    /// <inheritdoc />
    public bool FolderExists(string path)
    {
        if (string.IsNullOrEmpty(path) || path == Root.Path)
            return true;

        if (!_archive.TryGetBlock(path, out _)) 
            return false;

        return true;
    }

    /// <inheritdoc />
    public IFile GetFile(string path)
    {
        AssertFileExists(path);

        var entry = _archive.GetEntry(path);

        return new PackFile.PackFile(path, entry!, this);
    }

    /// <inheritdoc />
    public IFolder GetFolder(string path)
    {
        AssertFolderExists(path);

        var folderEntry = _archive.GetEntry(path)!;
        
        return new PackFolder(path, folderEntry, this);
    }

    /// <inheritdoc />
    public IEnumerable<IFile> GetFiles(string folderPath)
    {
        AssertFolderExists(folderPath);

        if (!_archive.TryGetBlock(folderPath, out var block)) 
            return Array.Empty<IFile>();
        
        var entries = block!.GetEntries().Where(e => e.Type == PackEntryType.File).ToArray();

        var result = new Span<IFile>();

        for (var iFile = 0; iFile < entries.Length; iFile++)
        {
            var file = entries[iFile];
            
            result[iFile] = new PackFile.PackFile(folderPath + file.Name, file, this);
        }
        
        return result.ToArray();
    }

    /// <inheritdoc />
    public IFolder[] GetFolders(string folderPath)
    {
        AssertFolderExists(folderPath);

        if (!_archive.TryGetBlock(folderPath, out var block)) return Array.Empty<IFolder>();
        
        var entries = block!.GetEntries().Where(e => e.Type == PackEntryType.Folder).ToArray();

        var result = new Span<IFolder>();

        for (var iFolder = 0; iFolder < entries.Length; iFolder++)
        {
            var folder = entries[iFolder];
            var currentFolderPath = PathUtil.Append(folderPath, folder.Name);

            result[iFolder] = new PackFolder(currentFolderPath, folder, this);
        }
        
        return result.ToArray();
    }

    /// <inheritdoc />
    public string[] GetChildren(string folderPath)
    {
        AssertFolderExists(folderPath);
        
        if (!_archive.TryGetBlock(folderPath, out var block)) 
            return Array.Empty<string>();
        
        var entries = block!.GetEntries().Where(e => e.Type != PackEntryType.Nop && !e.IsNavigator()).ToArray();

        var result = new Span<string>();

        for (var iEntry = 0; iEntry < entries.Length; iEntry++)
        {
            var entry = entries[iEntry];
            
            var entryPath = PathUtil.Append(folderPath, entry.Name);
            result[iEntry] = entryPath;
        }
        
        return result.ToArray();
    }

    /// <inheritdoc />
    public IFileReader OpenRead(string path)
    {
        AssertFileExists(path);

        var entry = _archive.GetEntry(path);
        if (entry is not { Type: PackEntryType.File })
            throw new FileNotFoundException($"The file {path} does not exist!");

        var bsRead = new BsReader(_fileStream);
        bsRead.BaseStream.Position = entry.DataPosition;
        var buffer = bsRead.ReadBytes(entry.Size);

        return new PackFileReader(new MemoryStream(buffer));
    }
    
    public void Dispose()
    {
        _fileStream.Close();
        _fileStream.Dispose();
    }
    
    #endregion
    
    
    private void AssertFileExists(string path)
    {
        if (!FileExists(path))
            throw new FileNotFoundException($"The file {path} does not exist.");
    }

    private void AssertFolderExists(string path)
    {
        if (path != Root.Path && !FolderExists(path))
            throw new DirectoryNotFoundException($"The folder {path} does not exist.");
    }
}