using RB.Core.FileSystem.IO;

namespace RB.Core.FileSystem.Local.IO;

public class LocalFileWriter : IFileWriter
{
    private readonly FileStream _stream;

    public LocalFileWriter(FileStream stream)
    {
        _stream = stream;
    }

    public void Write(byte[] data)
    {
        _stream.Write(data);
    }

    public void Write(byte[] data, int position)
    {
        _stream.Position = position;
        _stream.Write(data);
    }

    public Stream GetStream()
    {
        return _stream;
    }
}