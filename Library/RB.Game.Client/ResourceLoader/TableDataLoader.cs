using RB.Game.Client.Extensions;
using RB.Game.Objects.RefObject;
using RB.Game.Parser;

namespace RB.Game.Client.ResourceLoader;

public class TableDataLoader<TLoadResult> : ResourceLoader<TLoadResult> where TLoadResult : ILoadResult
{
    protected TableDataLoader(IClientFileSystem clientFileSystem) : base(clientFileSystem)
    {
    }

    protected void ReadList<TRefObj>(string fileName, ICollection<TRefObj> list) where TRefObj : IReferenceObj, new()
    {
        ReadList(GetFileFromMedia(fileName).GetStream(), list);
    }

    protected void ReadList<TRefObj>(Stream fileStream, ICollection<TRefObj> list) where TRefObj : IReferenceObj, new()
    {
        using var streamReader = new StreamReader(fileStream);
        while (!streamReader.EndOfStream)
        {
            if (!ReadNextLine(streamReader, out var line)) continue;

            var reference = new TRefObj();
            if (reference.Load(new RefObjectParser(line)))
                list.Add(reference);
        }
    }

    protected void ReadDictionary<TKey, TRefObj>(Stream fileStream, IDictionary<TKey, TRefObj> table)
        where TRefObj : IReferenceObj<TKey>, new()
    {
        using var streamReader = new StreamReader(fileStream);
        while (!streamReader.EndOfStream)
        {
            if (!ReadNextLine(streamReader, out var line))
                continue;

            var reference = new TRefObj();
            if (reference.Load(new RefObjectParser(line)))
                table[reference.PrimaryKey] = reference;
        }
    }

    protected void ReadDictionary<TKey, TRefObj>(string fileName, IDictionary<TKey, TRefObj> table)
        where TRefObj : IReferenceObj<TKey>, new()
    {
        ReadDictionary(GetFileFromMedia(fileName).GetStream(), table);
    }

    protected bool ReadNextLine(StreamReader reader, out string line)
    {
        line = reader.ReadLineByCrlf();

        if (string.IsNullOrEmpty(line) || line.StartsWith("//"))
            return false;

        return true;
    }

    public override bool TryLoad(string path, out TLoadResult result)
    {
        throw new NotImplementedException();
    }
}