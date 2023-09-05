using System.Text;
using RB.Core.FileSystem.PackFile.Cryptography;
using RB.Core.FileSystem.PackFile.Struct;

namespace RB.Core.FileSystem.PackFile.Component;

internal static class PackObjectFactory
{
    public static PackBlock GetRootBlock(long blockPosition = 256)
    {
        var result = GetEmptyBlock(blockPosition, blockPosition, true, true);

        return result;
    }

    public static PackBlock GetEmptyBlock(long blockPosition, long parentBlockPosition = 0, bool isFolder = false,
        bool isRoot = false, int parentFolderId = 0)
    {
        var result = new PackBlock
        {
            Id = IdGenerator.NextBlockId(),
            Entries = new PackEntry[20],
            ParentFolderId = parentFolderId,
            Position = blockPosition
        };

        //In case of a folder Navigate up/down entries
        if (isFolder)
        {
            result.Entries[0] = GetDotFolder(result.Id, blockPosition, parentFolderId);

            //The root folder doesn't have a go up navigator ".."
            if (isRoot)
                result.Entries[1] = GetNopEntry(result.Id);
            else
                result.Entries[1] = GetDotDotFolder(result.Id, parentBlockPosition, parentFolderId);
        }

        //In case of a continuation of a block most likely
        else
        {
            result.Entries[0] = GetNopEntry(result.Id);
            result.Entries[1] = GetNopEntry(result.Id);
        }

        for (var i = 2; i < 20; i++)
            result.Entries[i] = GetNopEntry(result.Id);

        return result;
    }

    public static PackHeader GetHeader(Blowfish? blowfish)
    {
        return new PackHeader
        {
            Encrypted = blowfish == null ? (byte)0 : (byte)1,
            EncryptionChecksum = blowfish?.Encode(Encoding.Default.GetBytes(PackHeader.BlowfishChecksumDecoded)) ??
                                 new byte[16],
            Signature = "JoyMax File Manager!\n".ToCharArray(),
            Version = 0x01000002
        };
    }

    public static PackBlock GetEmptyBlock(PackBlock parent)
    {
        return GetEmptyBlock(parent.Id, parent.Position);
    }

    public static PackEntry GetFileEntry(string name, int size, long dataPosition, int blockId, int parentFolderId)
    {
        return new PackEntry
        {
            BlockId = blockId,
            Id = IdGenerator.NextEntryId(),
            CreationTime = DateTime.Now,
            ModifyTime = DateTime.Now,
            DataPosition = dataPosition,
            Type = PackEntryType.File,
            Name = name,
            NextBlock = 0,
            ParentFolderId = parentFolderId,
            Payload = new byte[2],
            Size = size
        };
    }

    public static PackEntry GetFolderEntry(string name, int blockId, int parentFolderId, long folderBlockPosition)
    {
        return new PackEntry
        {
            BlockId = blockId,
            Id = IdGenerator.NextEntryId(),
            Name = name,
            CreationTime = DateTime.Now,
            ModifyTime = DateTime.Now,
            DataPosition = folderBlockPosition,
            NextBlock = 0, //ToDo: Rethink this in case the block exceeds the 20 entries
            ParentFolderId = parentFolderId,
            Payload = new byte[2],
            Type = PackEntryType.Folder,
            Size = 0
        };
    }

    public static PackEntry GetNopEntry(int blockId)
    {
        return new PackEntry
        {
            BlockId = blockId,
            Id = IdGenerator.NextEntryId(),
            CreationTime = DateTime.Now,
            ModifyTime = DateTime.Now,
            DataPosition = 0,
            Type = PackEntryType.Nop,
            Name = string.Empty,
            NextBlock = 0,
            ParentFolderId = 0,
            Payload = new byte[2],
            Size = 0
        };
    }

    public static PackEntry GetDotFolder(int blockId, long parentFolderPosition, int folderId)
    {
        return new PackEntry
        {
            BlockId = blockId,
            Id = IdGenerator.NextEntryId(),
            CreationTime = DateTime.Now,
            ModifyTime = DateTime.Now,
            DataPosition = parentFolderPosition,
            Type = PackEntryType.Folder,
            Name = ".",
            NextBlock = 0,
            ParentFolderId = folderId,
            Payload = new byte[2],
            Size = 0
        };
    }

    public static PackEntry GetDotDotFolder(int blockId, long parentBlockPosition, int folderId)
    {
        return new PackEntry
        {
            BlockId = blockId,
            Id = IdGenerator.NextEntryId(),
            CreationTime = DateTime.Now,
            ModifyTime = DateTime.Now,
            DataPosition = parentBlockPosition,
            Type = PackEntryType.Folder,
            Name = "..",
            NextBlock = 0,
            ParentFolderId = folderId,
            Payload = new byte[2],
            Size = 0
        };
    }
}