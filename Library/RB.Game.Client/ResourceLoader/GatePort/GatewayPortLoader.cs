using System.Diagnostics;
using System.Globalization;
using System.Text;
using RB.Core.FileSystem.PackFile.Cryptography;
using Serilog;

namespace RB.Game.Client.ResourceLoader.GatePort;

public class GatewayPortLoader : ResourceLoader<GatewayPortLoaderResult, ushort>, IGatewayPortLoader
{
    public GatewayPortLoader(IClientFileSystem clientFileSystem) : base(clientFileSystem)
    {
    }
    
    public bool TryLoad(out GatewayPortLoaderResult result)
    {
        return TryLoad(IGatewayPortLoader.Path, out result);
    }
    
    public override bool TryLoad(string path, out GatewayPortLoaderResult result)
    {
        try
        {
            OnLoading(path);
            
            var sw = Stopwatch.StartNew();
            
            var stream = ReadFileFromMedia(path).ReadAllText();
            ushort.TryParse(stream, out ushort port);
            
            result = new GatewayPortLoaderResult(true, path, port);
            
            OnLoaded(result);

            Log.Debug($"Loaded resource [{path}] in {sw.ElapsedMilliseconds}ms");
            
            return true;
        }
        catch (Exception e)
        {
            result = new GatewayPortLoaderResult(false, IGatewayPortLoader.Path, 0, e.Message);
            
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