using RB.Core.FileSystem.PackFile.Component;
using RB.Core.FileSystem.PackFile.Struct;

namespace RB.Core.FileSystem.PackFile;

public partial class PackWriter : IDisposable
{
    public PackWriter(FileStream fileStream, PackArchive archive)
    {
        FileStream = fileStream;
        Archive = archive;
    }

    /// <summary>
    ///     Gets the file stream for this instance.
    /// </summary>
    public FileStream FileStream { get; }

    /// <summary>
    ///     Gets the archive for this instance.
    /// </summary>
    public PackArchive Archive { get; }

    /// <summary>
    ///     Disposes the pack writer and the underlying file stream.
    /// </summary>
    public void Dispose()
    {
        Close();

        FileStream.Dispose();
    }

    public PackEntry DuplicateFileEntry(PackEntry sourceEntry, PackBlock destinationBlock, bool reindex = true)
    {
        return DuplicateFileEntry(sourceEntry, destinationBlock, sourceEntry.Name, reindex);
    }

    public PackEntry DuplicateFileEntry(PackEntry sourceEntry, PackBlock destinationBlock, string newName,
        bool reindex = true)
    {
        var block = GetOrCreateBlock(destinationBlock);
        var existingEntryIndex = Array.FindIndex(block.Entries, e => e.Type == PackEntryType.Nop);

        var newFileEntry = PackObjectFactory.GetFileEntry(newName, sourceEntry.Size, sourceEntry.DataPosition,
            destinationBlock.Id, destinationBlock.ParentFolderId);

        block.Entries[existingEntryIndex] = newFileEntry;

        UpdateBlocks(block);

        if (reindex)
            Archive.CreateIndex();

        return newFileEntry;
    }

    public PackEntry CreateFileEntry(string name, byte[] data, PackBlock parent, PackEntry parentFolder,
        bool reindex = true)
    {
        var block = GetOrCreateBlock(parent);
        var existingEntryIndex = Array.FindIndex(block.Entries, e => e.Type == PackEntryType.Nop);

        var newFileEntry =
            PackObjectFactory.GetFileEntry(name, data.Length, FileStream.Length, block.Id, parentFolder.Id);
        parent.Entries[existingEntryIndex] = newFileEntry;

        //Write data back to pk2
        var fileDataPosition = AllocateFileSpace(FileStream, data.Length);

        UpdateBlocks(block, parent);
        WriteDataAt(FileStream, data, fileDataPosition);

        if (reindex)
            Archive.CreateIndex();

        return newFileEntry;
    }

    public PackEntry CreateFolderEntry(string name, PackBlock parent, PackEntry parentFolder, bool reindex = true)
    {
        var block = GetOrCreateBlock(parent);
        var existingEntryIndex = Array.FindIndex(block.Entries, e => e.Type == PackEntryType.Nop);

        var newFolderBlock = PackObjectFactory.GetEmptyBlock(FileStream.Length, parent.Position, true);
        var newFolderEntry =
            PackObjectFactory.GetFolderEntry(name, newFolderBlock.Id, parentFolder.Id, newFolderBlock.Position);

        newFolderBlock.Entries[0].ParentFolderId = newFolderEntry.Id;
        newFolderBlock.Entries[1].ParentFolderId = newFolderEntry.Id;
        newFolderBlock.ParentFolderId = newFolderEntry.Id;
        
        parent.Entries[existingEntryIndex] = newFolderEntry;

        UpdateBlocks(block, newFolderBlock);

        if (reindex)
            Archive.CreateIndex();

        return newFolderEntry;
    }

    public void DeleteEntry(PackBlock block, PackEntry entry, bool eraseFileData = true, bool reindex = true)
    {
        var entryIndex = Array.FindIndex(block.Entries, e => e.Id == entry.Id);

        if (entryIndex == -1)
            throw new IOException($"The entry {entry.Id} can not be deleted: Failed to find entry in block.");

        var existingEntry = block.Entries[entryIndex];
        block.Entries[entryIndex] = PackObjectFactory.GetNopEntry(block.Id);

        //Overwrite file data with 00 00 00?
        if (eraseFileData && existingEntry.Type == PackEntryType.File)
            AllocateFileSpace(FileStream, existingEntry.Size);

        //Recursively call delete entry in case of sub folders and files
        if (existingEntry.Type == PackEntryType.Folder && !existingEntry.IsNavigator())
        {
            foreach (var childEntry in Archive.GetEntries(existingEntry.Id))
            {
                var entryBlock = Archive.GetBlock(childEntry.BlockId);
                if (entryBlock == null)
                    continue;
                
                DeleteEntry(entryBlock, childEntry, eraseFileData, false);
            }
        }
        
        UpdateBlocks(block);

        if (reindex)
            Archive.CreateIndex();
    }

    public void MoveEntry(PackBlock sourceBlock, PackEntry sourceEntry, PackBlock destinationBlock,
        int destinationFolderId, string newName, bool reindex = true)
    {
        var existingEntryIndex = Array.FindIndex(sourceBlock.Entries, e => e.Id == sourceEntry.Id);
        if (existingEntryIndex == -1)
            throw new IOException($"The entry {sourceEntry.Id} does not exist in the block {sourceBlock.Id}");

        //Nop old entry inside the block
        sourceBlock.Entries[existingEntryIndex] = PackObjectFactory.GetNopEntry(sourceBlock.Id);

        //Get destination block 
        destinationBlock = GetOrCreateBlock(destinationBlock);

        //Find the next nop entry in the destination block
        existingEntryIndex = Array.FindIndex(destinationBlock.Entries, e => e.Type == PackEntryType.Nop);
        if (existingEntryIndex == -1)
            throw new IOException($"The block {destinationBlock.Id} does not have nop entries.");

        //Update folder references
        destinationBlock.Entries[existingEntryIndex] = sourceEntry with
        {
            ParentFolderId = destinationFolderId,
            BlockId = destinationBlock.Id,
            Name = newName
        };

        UpdateBlocks(sourceBlock, destinationBlock);

        if (reindex)
            Archive.CreateIndex();
    }

    /// <summary>
    ///     Writes each provided block to the pack file and updates the references in the pack archive.
    /// </summary>
    /// <param name="blocks">The block to update.</param>
    private void UpdateBlocks(params PackBlock[] blocks)
    {
        foreach (var block in blocks)
        {
            WriteBlock(Archive.Blowfish, FileStream, block);

            Archive.AddOrUpdateBlock(block);
        }
    }

    /// <summary>
    ///     Gets the next usable block in a chain starting from the given startBlock.
    ///     If no nop entry is left it will automatically create a new block and return it to the caller.
    /// </summary>
    /// <param name="startBlock">The block to start the chain.</param>
    /// <returns>A pack block that can be used to store new entries.</returns>
    /// <exception cref="IOException">
    /// </exception>
    private PackBlock GetOrCreateBlock(PackBlock startBlock)
    {
        var parentBlockIndex = Array.FindIndex(Archive!.Blocks, b => b.Id == startBlock.Id);
        if (parentBlockIndex == -1)
            throw new IOException("The pack entry can not be created because the startBlock block is unknown.");

        var nextBlockPosition = startBlock.Entries[19].NextBlock;
        var existingEntryIndex = Array.FindIndex(startBlock.Entries, e => e.Type == PackEntryType.Nop);

        //The startBlock still has entries left that can be used
        if (existingEntryIndex != -1)
            return startBlock;

        if (nextBlockPosition > 0)
        {
            var nextBlock = Archive.GetBlockAt(nextBlockPosition);
            if (nextBlock == null)
                throw new IOException("The startBlock block has an unexpected ending.");

            return GetOrCreateBlock(nextBlock);
        }

        nextBlockPosition = FileStream.Length;

        //The block does not have any nop entries left, need to create a new chain.
        //Create a new block at the end of the file.
        var newBlock = PackObjectFactory.GetEmptyBlock(nextBlockPosition); //eof
        startBlock.Entries[19].NextBlock = nextBlockPosition;

        UpdateBlocks(newBlock, startBlock);

        return newBlock;
    }

    /// <summary>
    ///     Closes the file stream.
    /// </summary>
    public void Close()
    {
        FileStream.Flush();
        FileStream.Close();
    }
}