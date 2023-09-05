using RB.Core.FileSystem.PackFile.Component;
using RB.Core.FileSystem.PackFile.Cryptography;
using RB.Core.FileSystem.PackFile.Struct;

namespace RB.Core.FileSystem.PackFile;

public class PackArchive
{
    public const int ShadowRootId = 0;
    public const string ShadowRootName = "";
    public const string ShadowRootPath = "";

    #region Properties

    /// <summary>
    ///     Gets the character that determines path separation.
    /// </summary>
    public char PathSeparator { get; }

    /// <summary>
    ///     Gets the header of this pack file.
    /// </summary>
    public PackHeader Header { get; }

    /// <summary>
    ///     Gets the collection of blocks in this pack file.
    /// </summary>
    public PackBlock[] Blocks { get; private set; }

    /// <summary>
    ///     Gets the lookup table for this pack file.
    /// </summary>
    public PackFileLookupTable LookupTable { get; }

    /// <summary>
    ///     Gets the cryptographic blowfish for this pack file.
    ///     Can be null if the pack file is not encrypted.
    /// </summary>
    public Blowfish? Blowfish { get; }

    #endregion

    /// <summary>
    ///     Creates a new instance of <see cref="PackArchive" />
    /// </summary>
    /// <param name="header">The header of the pack file.</param>
    /// <param name="blocks">The collection of blocks in the pack file.</param>
    /// <param name="blowfish">The cryptographic blowfish for this pack file or null if the file is not encrypted.</param>
    /// <param name="pathSeparator">The character to determine path separation.</param>
    public PackArchive(PackHeader header, PackBlock[] blocks, Blowfish? blowfish, char pathSeparator = '\\')
    {
        Header = header;
        Blocks = blocks;
        LookupTable = new PackFileLookupTable(this);
        Blowfish = blowfish;
        PathSeparator = pathSeparator;

        CreateIndex();
    }

    #region Methods

    /// <summary>
    ///     Returns an entry
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public PackEntry? GetEntry(int id)
    {
        if (id == ShadowRootId)
            return GetShadowRoot();

        return Blocks.SelectMany(block => block.Entries).FirstOrDefault(entry => entry.Id == id);
    }

    /// <summary>
    ///     Returns a pack entry by its path.
    /// </summary>
    /// <param name="path">The path to the pack entry.</param>
    /// <returns>The found pack entry or null if the path does not exist.</returns>
    public PackEntry? GetEntry(string path)
    {
        if (path is ShadowRootPath or null)
            return GetShadowRoot();

        if (!LookupTable.ContainsKey(path))
            return null;

        var entryId = LookupTable[path];

        return GetEntry(entryId);
    }

    /// <summary>
    ///     Lookups up the given path and returns the entry including the entries block.
    /// </summary>
    /// <param name="path">The path of the entry to lookup.</param>
    /// <param name="entry">The found entry or null if the entry does not exist.</param>
    /// <param name="block">The found block or null if the block does not exist</param>
    /// <returns>True if the entry and block both exist. False if the entry or block doesn't exist.</returns>
    public bool TryGetEntryWithBlock(string path, out PackEntry? entry, out PackBlock? block)
    {
        block = null;
        entry = GetEntry(path);

        if (entry == null)
            return false;

        block = GetBlock(entry.BlockId);

        return block != null;
    }

    /// <summary>
    ///     Returns the pack block with the given id or null if the block does not exist.
    /// </summary>
    /// <param name="id">The identifier of the block to lookup.</param>
    /// <returns>The pack block or null if the block does not exist.</returns>
    public PackBlock? GetBlock(int id)
    {
        if (id == ShadowRootId)
            return Blocks[0];

        return Blocks.FirstOrDefault(b => b.Id == id);
    }

    /// <summary>
    ///     Returns a collection of child entries for the given parent identifier.
    /// </summary>
    /// <param name="parentId">The identifier of the parent folder entry.</param>
    /// <returns></returns>
    public IEnumerable<PackEntry> GetEntries(int parentId)
    {
        return Blocks.SelectMany(block =>
            block.Entries.Where(e => e.ParentFolderId == parentId && e.Type != PackEntryType.Nop));
    }

    /// <summary>
    ///     Returns a collection of child entries for the given path.
    /// </summary>
    /// <param name="path">The path of the folder.</param>
    /// <returns></returns>
    public IEnumerable<PackEntry> GetEntries(string path)
    {
        var entry = GetEntry(path);

        return entry == null ? Array.Empty<PackEntry>() : GetEntries(entry.Id);
    }

    /// <summary>
    ///     Returns the first block that points to the given position inside the pack file.
    /// </summary>
    /// <param name="position">The position of the block inside the pack file.</param>
    /// <returns>The block at the position or null if there is not block at the given position</returns>
    public PackBlock? GetBlockAt(long position)
    {
        return Blocks.FirstOrDefault(b => b.Position == position);
    }

    /// <summary>
    ///     Adds a new block to this pack archive.
    /// </summary>
    /// <param name="block">The block to add.</param>
    public void AddBlock(PackBlock block)
    {
        var newBlockData = new PackBlock[Blocks.Length + 1]; //+1 for the new block
        Blocks.CopyTo(newBlockData, 0);

        newBlockData[Blocks.Length] = block;

        Blocks = newBlockData;
    }

    /// <summary>
    ///     Adds or updates a block in this pack archive.
    /// </summary>
    /// <param name="updatedBlock">The pack block that should be created or updated.</param>
    public void AddOrUpdateBlock(PackBlock updatedBlock)
    {
        var blockIndex = Array.FindIndex(Blocks, b => b.Id == updatedBlock.Id);

        if (blockIndex == -1)
        {
            AddBlock(updatedBlock);

            return;
        }

        Blocks[blockIndex] = updatedBlock;
    }

    /// <summary>
    ///     Refreshes the current file lookup table.
    /// </summary>
    public void CreateIndex()
    {
        LookupTable.CreateLookupTable();
    }

    /// <summary>
    ///     Returns the default shadow root entry. The entry points to the root block at position 256.
    /// </summary>
    /// <returns></returns>
    private PackEntry GetShadowRoot()
    {
        return new PackEntry
        {
            BlockId = Blocks[0].Id,
            CreationTime = DateTime.Now,
            DataPosition = 256,
            Id = ShadowRootId,
            ModifyTime = DateTime.Now,
            Name = ShadowRootName,
            NextBlock = 0,
            ParentFolderId = 0,
            Payload = new byte[2],
            Size = 0,
            Type = PackEntryType.Folder
        };
    }

    #endregion
}