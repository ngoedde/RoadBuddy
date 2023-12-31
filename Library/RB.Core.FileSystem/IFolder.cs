namespace RB.Core.FileSystem;

public interface IFolder
{
    #region Properties

    public string Path { get; }

    public string Name { get; }

    public IFolder Parent { get; }

    public IFileSystem FileSystem { get; }

    public DateTime CreationTime { get; }

    public DateTime ModifyTime { get; }

    #endregion

    #region Methods

    public IFolder[] GetFolders();
    public IEnumerable<IFile> GetFiles();
    public string[] GetChildren();

    #endregion
}