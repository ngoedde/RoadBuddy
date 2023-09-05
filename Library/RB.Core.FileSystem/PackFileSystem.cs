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
        Id = 0,
        BlockId = Archive.Blocks[0].Id,
        ParentFolderId = 0,
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

    /// <inheritdoc />
    public bool ReadOnly { get; }
    
    public PackArchive Archive { get; }
    
    public Encoding Encoding { get; set; } = Encoding.Default;

    #endregion
    
    #region Constructor
    
    private readonly FileStream _fileStream;
    private readonly PackWriter? _packFileWriter;
    
    public PackFileSystem(string path, bool readOnly)
    {
        ReadOnly = readOnly;
        BasePath = path;

        var packFileReader = new PackReader();

        _fileStream = !readOnly
            ? new FileStream(path, FileMode.Open, FileAccess.ReadWrite)
            : new FileStream(path, FileMode.Open, FileAccess.Read);

        Archive = packFileReader.Read(_fileStream, null,  PathSeparator);
        _packFileWriter = ReadOnly ? null : new PackWriter(_fileStream, Archive);

        PathUtil.PathSeparator = PathSeparator;
    }

    public PackFileSystem(string path, string password, byte[] salt, bool readOnly)
    {
        ReadOnly = readOnly;
        BasePath = path;

        _fileStream = !readOnly
            ? new FileStream(path, FileMode.Open, FileAccess.ReadWrite)
            : new FileStream(path, FileMode.Open, FileAccess.Read);

        var packFileReader = new PackReader();

        var key = BlowfishUtil.GenerateFinalBlowfishKey(password, salt);
        var blowfish = new Blowfish(key);

        Archive = packFileReader.Read(_fileStream, blowfish, PathSeparator);
        _packFileWriter = ReadOnly ? null : new PackWriter(_fileStream, Archive);
        
        PathUtil.PathSeparator = PathSeparator;
    }


    public PackFileSystem(string path, string password, bool readOnly) : this(path, password,
        new byte[] { 0x03, 0xF8, 0xE4, 0x44, 0x88, 0x99, 0x3F, 0x64, 0xFE, 0x35 }, readOnly)
    {
    }

    #endregion

    #region Implementations

    public static PackFileSystem Create(string path)
    {
        using var fileStream = File.OpenWrite(path);

        PackWriter.CreateNewPackFile(fileStream, null);

        fileStream.Close();

        return new PackFileSystem(path, false);
    }

    public static PackFileSystem Create(string path, string password)
    {
        return Create(path, password, new byte[] { 0x03, 0xF8, 0xE4, 0x44, 0x88, 0x99, 0x3F, 0x64, 0xFE, 0x35 });
    }

    public static PackFileSystem Create(string path, string password, byte[] salt)
    {
        var blowfish = new Blowfish(BlowfishUtil.GenerateFinalBlowfishKey(password, salt));

        using var fileStream = File.OpenWrite(path);

        PackWriter.CreateNewPackFile(fileStream, blowfish);

        fileStream.Close();

        return new PackFileSystem(path, password, salt, false);
    }
    
    /// <inheritdoc />
    public bool FileExists(string path)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        if (!Archive.LookupTable.ContainsKey(path))
            return false;

        var entry = Archive.GetEntry(path);

        return entry is { Type: PackEntryType.File };
    }

    /// <inheritdoc />
    public bool FolderExists(string path)
    {
        if (string.IsNullOrEmpty(path) || path == Root.Path)
            return true;

        if (!Archive.LookupTable.ContainsKey(path))
            return false;

        var entry = Archive.GetEntry(path);

        return entry is { Type: PackEntryType.Folder };
    }

    /// <inheritdoc />
    public IFile GetFile(string path)
    {
        AssertFileExists(path);

        var entry = Archive.GetEntry(path);

        return new PackFile.PackFile(path, entry!, this);
    }

    /// <inheritdoc />
    public IFolder GetFolder(string path)
    {
        AssertFolderExists(path);

        var folderEntry = Archive.GetEntry(path)!;
        
        return new PackFolder(path, folderEntry, this);
    }

    /// <inheritdoc />
    public IEnumerable<IFile> GetFiles(string folderPath)
    {
        AssertFolderExists(folderPath);

        var entries = Archive.GetEntries(folderPath).Where(e => e.Type == PackEntryType.File);
        var result = new List<IFile>();
        result.AddRange(from entry in entries
            let path = Archive.LookupTable.GetPathById(entry.Id)
            select new PackFile.PackFile(path, entry, this));

        return result.ToArray();
    }

    /// <inheritdoc />
    public IFolder[] GetFolders(string folderPath)
    {
        AssertFolderExists(folderPath);

        // var entries = Archive.GetEntries(folderPath).Where(e => e.Type == PackEntryType.Folder && !e.IsNavigator());
        var entries = Archive.GetEntries(folderPath).Where(e => e.Type == PackEntryType.Folder);
        var result = new List<IFolder>();
        result.AddRange(from entry in entries
            let path = Archive.LookupTable.GetPathById(entry.Id)
            select new PackFolder(path, entry, this));

        return result.ToArray();
    }

    /// <inheritdoc />
    public string[] GetChildren(string folderPath)
    {
        AssertFolderExists(folderPath);

        var entry = Archive.GetEntry(folderPath)!;
        var entries = Archive.GetEntries(entry.Id);

        var result = new List<string>();
        result.AddRange(entries.Select(packEntry => Archive.LookupTable.GetPathById(packEntry.Id))
            .Where(path => path != null)!);

        return result.ToArray();
    }

    /// <inheritdoc />
    public IFile CreateFile(string path, byte[] data)
    {
        if (string.IsNullOrEmpty(path))
            throw new IOException("The path can not be empty.");

        AssertWritable();
        AssertFileNotExist(path);

        var fileName = PathUtil.GetFileName(path);
        var parentFolderPath = PathUtil.GetFolderName(path);

        if (!Archive.TryGetEntryWithBlock(parentFolderPath, out var parent, out var parentBlock))
            throw new IOException($"The block for the folder {parentFolderPath} does not exist. ");

        var folderBlock = Archive.GetBlockAt(parent!.DataPosition);
        if (folderBlock == null)
            throw new IOException($"The data block for the folder {parentFolderPath} does not exist. ");
        
        var newEntry = _packFileWriter!.CreateFileEntry(fileName, data, folderBlock!, parent!);

        return new PackFile.PackFile(path, newEntry, this);
    }

    /// <inheritdoc />
    public IFile CreateFile(string path, string content)
    {
        AssertWritable();
        AssertFileNotExist(path);

        return CreateFile(path, Encoding.GetBytes(content));
    }

    /// <inheritdoc />
    public IFolder CreateFolder(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new IOException("The path can not be empty.");

        AssertWritable();
        AssertFileNotExist(path);
        AssertFolderNotExist(path);

        var parentFolderPath = PathUtil.GetFolderName(path);
        AssertFolderExists(parentFolderPath);

        if (!Archive.TryGetEntryWithBlock(parentFolderPath, out var parent, out var block))
            throw new IOException($"The block or the entry for the folder {parentFolderPath} does not exist.");
        
        var folderBlock = Archive.GetBlockAt(parent!.DataPosition) ?? block;

        var newFolderName = PathUtil.GetFileName(path);
        var newFolderEntry = _packFileWriter!.CreateFolderEntry(newFolderName, folderBlock!, parent);

        return new PackFolder(path, newFolderEntry, this);
    }

    /// <inheritdoc />
    public void Move(string path, string newPath)
    {
        AssertWritable();

        if (!FileExists(path) && !FolderExists(path))
            throw new IOException($"The entry path {path} does not exist.");

        if (!Archive.TryGetEntryWithBlock(path, out var sourceEntry, out var sourceBlock))
            throw new IOException("The block or entry does not exist.");

        var destinationParentPath = PathUtil.GetFolderName(newPath);
        AssertFolderExists(destinationParentPath);

        if (!Archive.TryGetEntryWithBlock(destinationParentPath, out var destinationFolder, out var destinationBlock))
            throw new IOException("The block of the new parent folder does not exist.");

        var newName = PathUtil.GetFileName(newPath);

        _packFileWriter!.MoveEntry(sourceBlock!, sourceEntry!, destinationBlock!, destinationFolder!.Id, newName);
    }

    /// <inheritdoc />
    public void Delete(string path)
    {
        AssertWritable();

        if (!FileExists(path) && !FolderExists(path))
            throw new IOException($"The entry path {path} does not exist.");

        if (!Archive.TryGetEntryWithBlock(path, out var sourceEntry, out var sourceBlock))
            throw new IOException("The entry itself or the block of the entry does not exist.");

        _packFileWriter!.DeleteEntry(sourceBlock!, sourceEntry!);
    }

    /// <inheritdoc />
    public IFile CopyFile(string path, string destinationPath)
    {
        AssertWritable();
        AssertFileExists(path);
        AssertFileNotExist(destinationPath);

        var destinationParentPath = PathUtil.GetFolderName(destinationPath);
        AssertFolderExists(destinationParentPath);

        if (!Archive.TryGetEntryWithBlock(destinationParentPath, out var destinationParent, out var destinationBlock))
            throw new IOException("The destination block for the file does not exist.");

        var fileName = PathUtil.GetFileName(destinationPath);
        var newFileEntry = _packFileWriter!.CreateFileEntry(fileName, OpenRead(path).ReadAllBytes(), destinationBlock!,
            destinationParent!);

        return new PackFile.PackFile(path, newFileEntry, this);
    }

    public IFile DuplicateFile(string path, string destinationPath)
    {
        AssertWritable();
        AssertFileExists(path);
        AssertFileNotExist(destinationPath);

        var destinationParentPath = PathUtil.GetFolderName(destinationPath);
        AssertFolderExists(destinationParentPath);

        if (!Archive.TryGetEntryWithBlock(destinationParentPath, out _, out var destinationBlock))
            throw new IOException("The destination block for the file does not exist.");

        var sourceEntry = Archive.GetEntry(path)!;
        var newFileEntry =
            _packFileWriter!.DuplicateFileEntry(sourceEntry, destinationBlock!, Path.GetFileName(destinationPath));

        return new PackFile.PackFile(path, newFileEntry, this);
    }

    /// <inheritdoc />
    public IFolder CopyFolder(string path, string destinationPath, bool recursive = true)
    {
        AssertWritable();
        AssertFolderExists(path);
        AssertFolderNotExist(destinationPath);
        AssertFileNotExist(destinationPath);

        if (!Archive.TryGetEntryWithBlock(path, out var sourceEntry, out var sourceBlock))
            throw new IOException($"The block or entry for path {path} do not exist.");

        var parentDestinationPath = PathUtil.GetFolderName(destinationPath);
        if (!Archive.TryGetEntryWithBlock(parentDestinationPath, out var destinationEntry, out var destinationBlock))
            throw new IOException($"The block or entry for path {path} do not exist.");

        if (GetChildren(path).Length > 0 && !recursive)
            throw new IOException($"The folder {path} is not empty.");

        CreateFolder(destinationPath);
        
        foreach (var subFolder in GetFolders(path))
            CopyFolder(subFolder.Path,  PathUtil.EndingPathSeparator(destinationPath) + subFolder.Name);

        foreach (var file in GetFiles(path))
        {
            var newFilePath = PathUtil.EndingPathSeparator(destinationPath) + file.Name;
            AssertFileNotExist(newFilePath);

            CreateFile(newFilePath, OpenRead(file.Path).ReadAllBytes());
        }

        return GetFolder(destinationPath);
    }

    /// <inheritdoc />
    public IFileWriter OpenWrite(string path)
    {
        AssertWritable();
        AssertFileExists(path);

        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IFileReader OpenRead(string path)
    {
        AssertFileExists(path);

        var entry = Archive.GetEntry(path);
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

    private void AssertFileNotExist(string path)
    {
        if (FileExists(path))
            throw new IOException($"The file {path} does already exist.");
    }

    private void AssertFolderNotExist(string path)
    {
        if (FolderExists(path))
            throw new IOException($"The folder {path} does already exist.");
    }

    private void AssertWritable()
    {
        if (ReadOnly)
            throw new IOException("The file system is read-only.");

        if (_packFileWriter == null)
            throw new IOException("The pack writer is not set.");
    }
}