namespace RB.Core.FileSystem.PackFile.Component;

internal static class IdGenerator
{
    private static int _lastGeneratedEntryId = 1;
    private static int _lastGeneratedBlockId = 1;

    /// <summary>
    ///     Generates the next id for a pack entry.
    /// </summary>
    /// <returns></returns>
    public static int NextEntryId()
    {
        return _lastGeneratedEntryId++;
    }

    /// <summary>
    ///     Generates the next id for a pack block.
    /// </summary>
    /// <returns></returns>
    public static int NextBlockId()
    {
        return _lastGeneratedBlockId++;
    }
}