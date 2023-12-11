namespace FlipsideCrypto.NET.Common;
public class ImpossibleException : Exception
{
    public ImpossibleException(string? message)
        : base(message)
    {
    }
}
