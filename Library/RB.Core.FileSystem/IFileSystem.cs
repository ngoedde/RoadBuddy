using RB.Core.FileSystem.IO;

namespace RB.Core.FileSystem;

public interface IFileSystem
{
    #region Properties

    /// <summary>
    ///     Gets the root element of this file system.
    /// </summary>
    public IFolder Root { get; }

    /// <summary>
    ///     Gets the base path of this file system.
    /// </summary>
    public string BasePath { get; }

    /// <summary>
    ///     Gets the path separator character of this file system.
    /// </summary>
    public char PathSeparator { get; }

    #endregion

    #region Methods

    public bool FileExists(string path);
    public bool FolderExists(string path);

    public bool FileExists(IFile file)
    {
        return FileExists(file.Path);
    }

    public bool FolderExists(IFolder folder)
    {
        return FolderExists(folder.Path);
    }

    public IFile GetFile(string path);
    public IFolder GetFolder(string path);

    public IEnumerable<IFile> GetFiles(string folderPath);
    public IFolder[] GetFolders(string folderPath);
    public string[] GetChildren(string folderPath);

    public IFileReader OpenRead(string path);

    public IEnumerable<byte> ReadAllBytes(string path)
    {
        return OpenRead(path).ReadAllBytes();
    }

    public byte[] Read(string path, int offset, int length)
    {
        return OpenRead(path).Read(offset, length);
    }

    public string ReadAllText(string path)
    {
        return OpenRead(path).ReadAllText();
    }

    public string[] ReadAllLines(string path)
    {
        return OpenRead(path).ReadAllText().Split(Environment.NewLine);
    }

    #endregion
}