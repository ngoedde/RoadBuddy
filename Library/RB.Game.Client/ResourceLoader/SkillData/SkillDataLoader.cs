using System.Diagnostics;
using RB.Core.FileSystem;
using RB.Game.Client.ResourceLoader.SkillData.Cryptography;
using RB.Game.Objects.RefObject;
using Serilog;

namespace RB.Game.Client.ResourceLoader.SkillData;

public class SkillDataLoader : TableDataLoader<SkillDataLoadResult>, ISkillDataLoader
{
    private readonly SkillCryptographyProvider _skillCryptographyProvider;

    public SkillDataLoader(IClientFileSystem clientFileSystem, SkillCryptographyProvider skillCryptographyProvider) : base(clientFileSystem)
    {
        _skillCryptographyProvider = skillCryptographyProvider;
    }

    public override bool TryLoad(string path, out SkillDataLoadResult result)
    {
        var skillData = new Dictionary<uint, RefSkill>(4096);

        try
        {
            var sw = Stopwatch.StartNew();
            var dataFileList = GetDataFileNames(path);

            foreach (var filePath in dataFileList)
            {
                using var stream = GetFileFromMedia(filePath).GetStream();
                
                _skillCryptographyProvider.DecryptStream(stream, out var decodedStream);
                
                ReadDictionary(decodedStream, skillData);
            }

            Log.Debug($"Loaded {skillData.Count} skills in {sw.ElapsedMilliseconds}ms");

            result = new SkillDataLoadResult(true, path, skillData);

            return true;
        }
        catch (Exception ex)
        {
            result = new SkillDataLoadResult(false, path, skillData, ex.Message);

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