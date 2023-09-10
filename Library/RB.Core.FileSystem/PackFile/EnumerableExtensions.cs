using RB.Core.FileSystem.PackFile.Struct;

namespace RB.Core.FileSystem.PackFile;

public static class EnumerableExtensions
{
    public static IEnumerable<PackEntry> GetEntries(this IEnumerable<PackBlock> blocks)
    {
        return blocks.SelectMany(block => block.Entries);
    }
    
    public static PackEntry? GetEntry(this IEnumerable<PackBlock> blocks, string name)
    {
        return GetEntries(blocks).FirstOrDefault(e => e.Name == name);
    }
}