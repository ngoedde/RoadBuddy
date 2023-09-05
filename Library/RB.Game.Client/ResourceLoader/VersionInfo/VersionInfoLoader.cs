using System.Text;
using RB.Core.FileSystem.PackFile.Cryptography;

namespace RB.Game.Client.ResourceLoader.VersionInfo;

public class VersionInfoLoader : ResourceLoader<VersionInfoLoaderResult, int>, IVersionInfoLoader
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
            base.OnLoading(path);
            
            var buffer = ReadFileFromMedia(path).ReadAllBytes();
            int version;
            
            using (var stream = new MemoryStream(buffer))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var versionBufferLength = reader.ReadInt32();
                    var versionBuffer = reader.ReadBytes(versionBufferLength);

                    var blowfish = new Blowfish();
                    blowfish.Initialize(Encoding.ASCII.GetBytes("SILKROADVERSION"), 0, 8);

                    var decodedVersionBuffer = blowfish.Decode(versionBuffer);
                    version = int.Parse(Encoding.ASCII.GetString(decodedVersionBuffer, 0, 4));
                }
            }
        
            result = new VersionInfoLoaderResult(true, path, version);
            
            base.OnLoaded(result);

            return true;
        }
        catch (Exception e)
        {
            result = new VersionInfoLoaderResult(false, IVersionInfoLoader.Path, 0, e.Message);
            
            return false;
        }
    }
}