namespace RB.Core.FileSystem.PackFile.Struct;

public record PackEntry
{
    /// <summary>
    ///     The id of the block this entry belongs to.
    /// </summary>
    public int BlockId;

    /// <summary>
    ///     The creation timestamp of this entry.
    /// </summary>
    public DateTime CreationTime;

    /// <summary>
    ///     The data position of this entry.
    ///     In case of an folder it's the block position of the entry.
    ///     In case of a file it's the file data position of the entry
    /// </summary>
    public long DataPosition;

    /// <summary>
    ///     The identifier of this entry.
    /// </summary>
    public int Id;

    /// <summary>
    ///     The modification timestamp of this entry.
    /// </summary>
    public DateTime ModifyTime;

    /// <summary>
    ///     The name of this entry.
    /// </summary>
    public string Name = string.Empty;

    /// <summary>
    ///     The position of the next block for this entry.
    /// </summary>
    public long NextBlock;

    /// <summary>
    ///     The id of the folder this entry belongs to.
    /// </summary>
    public int ParentFolderId;

    /// <summary>
    ///     The payload of this entry. Can be used to store additional meta information to an entry.
    /// </summary>
    public byte[] Payload = new byte[2];

    /// <summary>
    ///     The size of this entry.
    /// </summary>
    public int Size;

    /// <summary>
    ///     The type of this entry.
    /// </summary>
    public PackEntryType Type;

    /// <summary>
    ///     Returns a value indicating if this entry is a sub folder.
    /// </summary>
    /// <returns></returns>
    public bool IsSubFolder()
    {
        return Type == PackEntryType.Folder &&
               !string.IsNullOrEmpty(Name) &&
               DataPosition > 0;
    }

    /// <summary>
    ///     Returns a value indicating if this entry can be used for navigation through the pack file.
    /// </summary>
    /// <returns></returns>
    public bool IsNavigator()
    {
        return Type == PackEntryType.Folder && Name is "." or "..";
    }
}