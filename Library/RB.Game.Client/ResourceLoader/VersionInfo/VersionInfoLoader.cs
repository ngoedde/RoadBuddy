using System.Diagnostics;
using System.Text;
using RB.Core.FileSystem.PackFile.Cryptography;
using Serilog;

namespace RB.Game.Client.ResourceLoader.VersionInfo;

public class VersionInfoLoader : ResourceLoader<VersionInfoLoaderResult, uint>, IVersionInfoLoader
{
    public VersionInfoLoader(IClientFileSystem clientFileSystem) : base(clientFileSystem)
    {
    }
    
    public bool TryLoad(out VersionInfoLoaderResult result)
    {
        return TryLoad(IVersionInfoLoader.Path, out result);
    }
    
    public override bool TryLoad(string path, out VersionInfoLoaderResult result)
    {
        try
        {
            OnLoading(path);
            
            var sw = Stopwatch.StartNew();
            
            var stream = ReadFileFromMedia(path).GetStream();
            var version = ReadFromStream(stream);
            
            result = new VersionInfoLoaderResult(true, path, version);
            
            OnLoaded(result);

            Log.Debug($"Loaded resource [{path}] in {sw.ElapsedMilliseconds}ms");
            
            return true;
        }
        catch (Exception e)
        {
            result = new VersionInfoLoaderResult(false, IVersionInfoLoader.Path, 0, e.Message);
            
            return false;
        }
    }

    private uint ReadFromStream(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        
        var versionBufferLength = reader.ReadInt32();
        var versionBuffer = reader.ReadBytes(versionBufferLength);

        var blowfish = new Blowfish();
        blowfish.Initialize(Encoding.ASCII.GetBytes("SILKROADVERSION"), 0, 8);

        var decodedVersionBuffer = blowfish.Decode(versionBuffer);
        return uint.Parse(Encoding.ASCII.GetString(decodedVersionBuffer, 0, 4));
    }
}