using RB.Game.Parser;

namespace RB.Game.Objects.RefObject;

public interface IReferenceObj
{
    bool Load(RefObjectParser parser);
}

public interface IReferenceObj<TKey> : IReferenceObj
{
    TKey PrimaryKey { get; }
}