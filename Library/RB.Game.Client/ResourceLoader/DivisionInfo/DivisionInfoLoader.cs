using System.Diagnostics;
using RB.Core.FileSystem.IO;
using RB.Game.Client.Objects;
using Serilog;

namespace RB.Game.Client.ResourceLoader.DivisionInfo;

public class DivisionInfoLoader : ResourceLoader<DivisionInfoLoadResult>, IDivisionInfoLoader
{
    public DivisionInfoLoader(IClientFileSystem clientFileSystem) : base(clientFileSystem)
    {
    }

    public bool TryLoad(out DivisionInfoLoadResult result)
    {
        return TryLoad(IDivisionInfoLoader.Path, out result);
    }

    public override bool TryLoad(string path, out DivisionInfoLoadResult result)
    {
        try
        {
            var sw = Stopwatch.StartNew();
            OnLoading(path);

            var buffer = GetFileFromMedia(IDivisionInfoLoader.Path).GetStream();
            var divisionInfo = ReadFromStream(buffer);

            result = new DivisionInfoLoadResult(true, path, divisionInfo);

            OnLoaded(result);

            Log.Debug($"Loaded resource [{path}] in {sw.ElapsedMilliseconds}ms");
            return true;
        }
        catch (Exception e)
        {
            result = new DivisionInfoLoadResult(false, path, null, e.Message);

            OnLoaded(result);

            return false;
        }
    }

    private Objects.DivisionInfo ReadFromStream(Stream stream)
    {
        using (var reader = new BsReader(stream))
        {
            var locale = reader.ReadByte();

            var divisionCount = reader.ReadByte();
            var divisions = new Division[divisionCount];

            for (var iDivision = 0; iDivision < divisionCount; iDivision++)
            {
                var name = reader.ReadString();
                reader.ReadByte(); //NOP

                var gatewayCount = reader.ReadByte();
                var gateways = new string[gatewayCount];

                for (var iGateway = 0; iGateway < gatewayCount; iGateway++)
                {
                    gateways[iGateway] = reader.ReadString();
                    reader.ReadByte(); //NOP
                }

                divisions[iDivision] = new Division(name, gateways);
            }

            return new Objects.DivisionInfo(locale, divisions);
        }
    }
}