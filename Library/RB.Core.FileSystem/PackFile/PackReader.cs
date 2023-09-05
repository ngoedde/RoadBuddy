using System.Diagnostics;
using System.Text;
using RB.Core.FileSystem.IO;
using RB.Core.FileSystem.PackFile.Component;
using RB.Core.FileSystem.PackFile.Cryptography;
using RB.Core.FileSystem.PackFile.Struct;

namespace RB.Core.FileSystem.PackFile;

internal class PackReader
{
    private Blowfish? _blowfish;

    public PackArchive Read(Stream fileStream, Blowfish? blowfish = null, char pathSeparator = '\\')
    {
        var sw = Stopwatch.StartNew();

        _blowfish = blowfish;

        var reader = new BsReader(fileStream);
        var header = ReadHeader(reader);

        //Check decryption key
        if (_blowfish != null && header.Encrypted == 0x01)
        {
            var tempChecksum = _blowfish.Encode(Encoding.ASCII.GetBytes(PackHeader.BlowfishChecksumDecoded));

            //Check if the security checksum equals the generated checksum
            if (tempChecksum == null || tempChecksum[0] != header.EncryptionChecksum[0]
                                     || tempChecksum[1] != header.EncryptionChecksum[1]
                                     || tempChecksum[2] != header.EncryptionChecksum[2])
                throw new IOException("Failed to open JoymaxPackFile: The password or salt is wrong.");
        }

        var blocks = ReadBlocksAt(reader, 256, 0);
        sw.Stop();

        Debug.WriteLine($"Reading pack file took {sw.ElapsedMilliseconds}ms");

        return new PackArchive(header, blocks.ToArray(), blowfish, pathSeparator);
    }

    public PackHeader ReadHeader(BinaryReader reader)
    {
        var result = new PackHeader
        {
            Signature = reader.ReadChars(30),
            Version = reader.ReadInt32(),
            Encrypted = reader.ReadByte(),
            EncryptionChecksum = reader.ReadBytes(16),
            Payload = reader.ReadBytes(205)
        };

        return result;
    }

    public IEnumerable<PackBlock> ReadBlocksAt(BinaryReader reader, long position, int parentFolderId)
    {
        var result = new List<PackBlock>(256);

        reader.BaseStream.Position = position;

        var blockId = IdGenerator.NextBlockId();
        var block = new PackBlock
        {
            Id = blockId,
            Position = position,
            ParentFolderId = parentFolderId,
            Entries = ReadEntries(reader, parentFolderId, blockId)
        };

        result.Add(block);

        //Read next block
        if (block.Entries[19].NextBlock > 0)
            result.AddRange(ReadBlocksAt(reader, block.Entries[19].NextBlock, parentFolderId));

        //Read sub folder blocks
        foreach (var subFolderEntry in block.Entries.Where(e => e.IsSubFolder() && !e.IsNavigator()))
            result.AddRange(ReadBlocksAt(reader, subFolderEntry.DataPosition, subFolderEntry.Id));

        return result;
    }

    private PackEntry[] ReadEntries(BinaryReader reader, int parentFolderId, int blockId)
    {
        var result = new PackEntry[20];

        //Read entries
        for (var iEntry = 0; iEntry < 20; iEntry++)
        {
            var entryId = IdGenerator.NextEntryId();
            var entryBuffer = reader.ReadBytes(128);

            if (_blowfish != null)
                entryBuffer = _blowfish.Decode(entryBuffer);

            using var entryReader = new BsReader(new MemoryStream(entryBuffer));
            result[iEntry] = new PackEntry
            {
                Id = entryId,
                ParentFolderId = parentFolderId,
                BlockId = blockId,
                Type = (PackEntryType)entryReader.ReadByte(),
                Name = entryReader.ReadString(89).Trim('\0'),
                CreationTime = DateTime.FromFileTimeUtc(entryReader.ReadInt64()),
                ModifyTime = DateTime.FromFileTimeUtc(entryReader.ReadInt64()),
                DataPosition = entryReader.ReadInt64(),
                Size = entryReader.ReadInt32(),
                NextBlock = entryReader.ReadInt64(),
                Payload = entryReader.ReadBytes(2) //Padding to reach 128 bytes length
            };
        }

        return result;
    }
}