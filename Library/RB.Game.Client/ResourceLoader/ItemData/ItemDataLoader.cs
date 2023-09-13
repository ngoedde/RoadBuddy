using System.Diagnostics;
using RB.Core.FileSystem;
using RB.Game.Objects.RefObject;
using Serilog;

namespace RB.Game.Client.ResourceLoader.ItemData;

public class ItemDataLoader : TableDataLoader<ItemDataLoadResult>, IItemDataLoader
{
    public ItemDataLoader(IClientFileSystem clientFileSystem) : base(clientFileSystem)
    {
    }

    public override bool TryLoad(string path, out ItemDataLoadResult result)
    {
        var itemData = new Dictionary<uint, RefObjItem>(4096);

        try
        {
            var sw = Stopwatch.StartNew();

            foreach (var itemDataFile in GetDataFileNames(path))
                ReadDictionary(itemDataFile, itemData);

            Log.Debug($"Loaded {itemData.Count} items in {sw.ElapsedMilliseconds}ms");

            result = new ItemDataLoadResult(true, path, itemData);

            return true;
        }
        catch (Exception ex)
        {
            result = new ItemDataLoadResult(false, path, itemData, ex.Message);

            return false;
        }
    }

    private IEnumerable<string> GetDataFileNames(string path)
    {
        var itemDataFileList = GetFileFromMedia(path).ReadAllLines();
        var folder = PathUtil.GetFolderName(path);

        return itemDataFileList
            .Where(f => !string.IsNullOrEmpty(f))
            .Select(f => PathUtil.Append(folder, f.Trim()));
    }
}