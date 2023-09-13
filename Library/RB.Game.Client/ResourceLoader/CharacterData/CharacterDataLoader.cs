using System.Diagnostics;
using RB.Core.FileSystem;
using RB.Game.Objects.RefObject;
using Serilog;

namespace RB.Game.Client.ResourceLoader.CharacterData;

public class CharacterDataLoader : TableDataLoader<CharacterDataLoadResult>, ICharacterDataLoader
{
    public CharacterDataLoader(IClientFileSystem clientFileSystem) : base(clientFileSystem)
    {
    }

    public override bool TryLoad(string path, out CharacterDataLoadResult result)
    {
        OnLoading(path);
        var characterData = new Dictionary<uint, RefObjChar>(4096);

        try
        {
            var sw = Stopwatch.StartNew();
            var itemDataFileList = GetDataFileNames(path);

            foreach (var itemDataFile in itemDataFileList)
                ReadDictionary(itemDataFile, characterData);
            
            Log.Debug($"Loaded {characterData.Count} characters in {sw.ElapsedMilliseconds}ms");

            result = new CharacterDataLoadResult(true, path, characterData);
            
            OnLoaded(result);
            return true;
        }
        catch (Exception ex)
        {
            result = new CharacterDataLoadResult(false, path, characterData, ex.Message);

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