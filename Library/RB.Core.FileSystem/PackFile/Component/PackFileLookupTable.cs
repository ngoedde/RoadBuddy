using System.Diagnostics;
using RB.Core.FileSystem.PackFile.Struct;

namespace RB.Core.FileSystem.PackFile.Component;

/// <summary>
///     Used to store paths to files and folders by their corresponding PackEntry-Id.
/// </summary>
public class PackFileLookupTable : Dictionary<string, int>
{
    private readonly PackArchive _archive;

    /// <summary>
    ///     Creates a new instance of <see cref="PackFileLookupTable" />.
    /// </summary>
    /// <param name="archive">The archive this lookup table belongs to.</param>
    /// <param name="caseSensitive"></param>
    public PackFileLookupTable(PackArchive archive) : base(1024)
    {
        _archive = archive;
    }

    /// <summary>
    ///     Creates the lookup table for the archive. Generates paths for the entries and adds them to this instance.
    /// </summary>
    public void CreateLookupTable()
    {
        var sw = Stopwatch.StartNew();
        Debug.WriteLine("PackFileLookupTable: Creating index...");

        Clear();

        var entries = _archive.Blocks
            .SelectMany(b => b.Entries.Where(e => !e.IsNavigator() && e.Type != PackEntryType.Nop))
            .OrderBy(e => e.Type); //Index folders first

        Debug.WriteLine($"PackFileLookupTable: Found {entries.Count()} pack file entries to index");

        //Add paths for each entry except navigation entries
        foreach (var entry in entries)
        {
            var path = GeneratePath(entry);

            if (string.IsNullOrEmpty(path) || ContainsKey(path))
                continue;

            TryAdd(path, entry.Id);
        }

        Debug.WriteLine($"PackFileLookupTable: Indexed {this.Count()} pack file entries in {sw.ElapsedMilliseconds}ms");
    }

    /// <summary>
    ///     Generates the path for the given entry.
    /// </summary>
    /// <param name="entry"></param>
    /// <returns>The absolute path to the entry</returns>
    private string GeneratePath(PackEntry entry)
    {
        var path = entry.Name;

        if (entry.ParentFolderId == 0)
            return entry.Name;

        var parentPath = GetPathById(entry.ParentFolderId);
        if (!string.IsNullOrEmpty(parentPath) && ContainsKey(parentPath))
            return parentPath + _archive.PathSeparator + entry.Name;

        var parentFolder = _archive.GetEntry(entry.ParentFolderId);

        while (parentFolder != null && parentFolder.ParentFolderId != 0)
        {
            path = $"{parentFolder.Name}{_archive.PathSeparator}{path}";
            parentFolder = _archive.GetEntry(parentFolder.ParentFolderId);
        }

        return path;
    }

    /// <summary>
    ///     Returns the path for the given entry.
    /// </summary>
    /// <param name="entryId">The identifier of the entry to lookup.</param>
    /// <returns>The path within the lookup table.</returns>
    public string? GetPathById(int entryId)
    {
        var entry = this.FirstOrDefault(l => l.Value == entryId);

        return entry.Value != 0 ? entry.Key : null;
    }
}