using RB.Core.FileSystem.IO;

namespace RB.Core.FileSystem;

public interface IFileSystem
{
    #region Properties

    /// <summary>
    ///     Gets a value indicating whether or not this file system is mounted as read-only.
    /// </summary>
    public bool ReadOnly { get; }

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

    public IFile CreateFile(string path, byte[] data);
    public IFile CreateFile(string path, string content);
    public IFolder CreateFolder(string path);

    public IFile CreateFile(IFile file, byte[] data)
    {
        return CreateFile(file.Path, data);
    }

    public IFile CreateFile(IFile file, string content)
    {
        return CreateFile(file.Path, content);
    }

    public IFolder CreateFolder(IFolder folder)
    {
        return CreateFolder(folder.Path);
    }

    public void Move(string path, string newPath);
    public void Delete(string path);

    public IFile CopyFile(string path, string destinationPath);
    public IFolder CopyFolder(string path, string destinationPath, bool recursive = true);

    public IFileWriter OpenWrite(string path);
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

    public void Write(string path, byte[] data)
    {
        OpenWrite(path).Write(data);
    }

    public void Write(string path, byte[] data, int position)
    {
        OpenWrite(path).Write(data, position);
    }

    #endregion
}