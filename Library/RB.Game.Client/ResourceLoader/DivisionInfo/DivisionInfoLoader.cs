using RB.Core.FileSystem.IO;
using RB.Game.Client.Objects;

namespace RB.Game.Client.ResourceLoader.DivisionInfo;

public class DivisionInfoLoader : ResourceLoader<DivisionInfoLoaderResult, Objects.DivisionInfo>, IDivisionInfoLoader
{
    public DivisionInfoLoader(IClientFileSystem clientFileSystem) : base(clientFileSystem)
    {
    }
    
    public bool TryLoad(out DivisionInfoLoaderResult result)
    {
        return TryLoad(IDivisionInfoLoader.Path, out result);
    }
    
    public override bool TryLoad(string path, out DivisionInfoLoaderResult result)
    {
        try
        {
            base.OnLoading(path);
   
            var buffer = ReadFileFromMedia(IDivisionInfoLoader.Path).GetStream();
            var divisionInfo = ReadFromStream(buffer);
            
            result = new DivisionInfoLoaderResult(true, path, divisionInfo);

            base.OnLoaded(result);
            
            return true;
        }
        catch (Exception e)
        {
            result = new DivisionInfoLoaderResult(false, path, null, e.Message);

            base.OnLoaded(result);
            
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