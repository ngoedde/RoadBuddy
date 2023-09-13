using RB.Core.FileSystem.IO;

namespace RB.Core.FileSystem;

public interface IFile
{
    #region Properties

    public string Path { get; }
    public string Name { get; }
    public string Extension { get; }
    public long Size { get; }
    public IFileSystem FileSystem { get; }
    public IFolder Parent { get; }

    public DateTime CreateTime { get; }
    public DateTime ModifyTime { get; }

    #endregion

    #region Methods

    public IFileReader OpenRead();


    public byte[] Read(int position, int length)
    {
        return OpenRead().Read(position, length);
    }

    public string ReadAllText()
    {
        return OpenRead().ReadAllText();
    }

    public byte[] ReadAllBytes()
    {
        return OpenRead().ReadAllBytes();
    }

    public string[] ReadAllLines()
    {
        return OpenRead().ReadAllText().Split(Environment.NewLine);
    }

    #endregion
}