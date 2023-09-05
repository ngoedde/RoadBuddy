using RB.Core.FileSystem.IO;

namespace RB.Core.FileSystem.PackFile.IO;

public class PackFileWriter : IFileWriter
{
    private readonly Stream _stream;

    public PackFileWriter(Stream stream)
    {
        if (!stream.CanWrite)
            throw new IOException("The file system is read-only.");

        _stream = stream;
    }

    /// <inheritdoc />
    public void Write(byte[] data)
    {
        if (data.Length > _stream.Length)
            throw new IOException("Can not write beyond file ending.");

        _stream.Write(data);
    }

    /// <inheritdoc />
    public void Write(byte[] data, int position)
    {
        if (position + data.Length > _stream.Length)
            throw new IOException("Can not write beyond file ending.");

        _stream.Write(data);
    }

    /// <inheritdoc />
    public Stream GetStream()
    {
        return _stream;
    }

    /// <inheritdoc />
    public void Write(string content)
    {
        var writer = new StreamWriter(_stream);

        writer.Write(content);
    }

    /// <inheritdoc />
    public void Write(string content, int position)
    {
        _stream.Position = position;

        var writer = new StreamWriter(_stream);
        writer.Write(content);
    }
}