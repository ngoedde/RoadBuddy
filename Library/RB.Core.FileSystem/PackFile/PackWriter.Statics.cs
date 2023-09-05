using RB.Core.FileSystem.PackFile.Component;
using RB.Core.FileSystem.PackFile.Cryptography;
using RB.Core.FileSystem.PackFile.Struct;

namespace RB.Core.FileSystem.PackFile;

public partial class PackWriter
{
    #region Static methods

    /// <summary>
    ///     Creates a new pack file using the given stream and cryptographic blowfish.
    /// </summary>
    /// <param name="stream">The file stream to write the data to.</param>
    /// <param name="blowfish">The cryptographic blowfish or null if the pack file is not encrypted.</param>
    public static void CreateNewPackFile(FileStream stream, Blowfish? blowfish)
    {
        WriteHeader(stream, PackObjectFactory.GetHeader(blowfish));
        WriteRootBlock(stream, blowfish);
    }

    /// <summary>
    ///     Writes the given header to the file stream.
    /// </summary>
    /// <param name="fileStream">The filestream to write the header to.</param>
    /// <param name="header">The header to write.</param>
    public static void WriteHeader(FileStream fileStream, PackHeader header)
    {
        fileStream.Position = 0;

        fileStream.Write(header.ToByteArray());
    }

    /// <summary>
    ///     Writes a new block to the filestream using the block's position property. If provided, the blowfish
    ///     will be used to encrypt the block before writing it to the filestream.
    /// </summary>
    /// <param name="blowfish">The cryptographic blowfish or null if the pack file is not encrypted.</param>
    /// <param name="fileStream">The filestream to write the data to.</param>
    /// <param name="block">The block to write.</param>
    public static void WriteBlock(Blowfish? blowfish, FileStream fileStream, PackBlock block)
    {
        var blockBuffer = blowfish != null ? blowfish.Encode(block.ToByteArray()) : block.ToByteArray();
        var completeBuffer =
            block.Position == 256 ? new byte[4096 - 256] : new byte[4096]; //256 for header in case of root block
        blockBuffer.CopyTo(completeBuffer, 0);

        fileStream.Position = block.Position;

        fileStream.Write(completeBuffer);
        fileStream.Flush(true);
    }

    /// <summary>
    ///     Writes the given file data to the filestream at the given position.
    /// </summary>
    /// <param name="fileStream">The filestream to write to.</param>
    /// <param name="fileData">The file data to write.</param>
    /// <param name="position">The position inside the pack file to write to.</param>
    public static void WriteDataAt(FileStream fileStream, byte[] fileData, long position)
    {
        fileStream.Position = position;

        fileStream.Write(fileData);
        fileStream.Flush(true);
    }

    /// <summary>
    ///     Writes the root block to the given filestream.
    /// </summary>
    /// <param name="stream">The filestream to write the root block to.</param>
    /// <param name="blowfish">
    ///     The cryptographic blowfish used to encrypt the root block. Null if the pack archive is not
    ///     encrypted.
    /// </param>
    public static void WriteRootBlock(FileStream stream, Blowfish? blowfish)
    {
        var block = PackObjectFactory.GetRootBlock();

        WriteBlock(blowfish, stream, block);
    }

    /// <summary>
    ///     Allocates free space for a file at the filestream.
    ///     It will allocate 4096K blocks until the file size has been reached.
    /// </summary>
    /// <param name="fileStream">The file stream to allocate free file space to.</param>
    /// <param name="fileSize">The size of the file to allocate.</param>
    /// <returns></returns>
    public static long AllocateFileSpace(FileStream fileStream, int fileSize)
    {
        var fileDataPosition = fileStream.Length; //eof
        var allocationArray = new byte[4096];

        //Always allocate 4KB
        if (fileSize < allocationArray.Length)
        {
            WriteDataAt(fileStream, allocationArray, fileDataPosition);

            return fileDataPosition;
        }

        while (fileSize >= allocationArray.Length || fileSize > 0)
        {
            WriteDataAt(fileStream, allocationArray, fileStream.Length); //eof

            fileSize -= 4096;
        }

        return fileDataPosition;
    }

    #endregion
}