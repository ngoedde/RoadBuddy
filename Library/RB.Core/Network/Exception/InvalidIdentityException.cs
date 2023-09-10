namespace RB.Core.Network.Exception;

public class InvalidIdentityException : System.Exception
{
    public string ExpectedIdentity { get; }
    public string ActualIdentity { get; }

    public override string Message =>
        $"Invalid NetEngine identity. Expected {ExpectedIdentity} but got {ActualIdentity}";

    public InvalidIdentityException(string expected, string actual)
    {
        ExpectedIdentity = expected;
        ActualIdentity = actual;
    }
}