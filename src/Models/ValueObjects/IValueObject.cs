namespace FlipsideCrypto.NET.Models.ValueObjects;
public interface IValueObject<TValueObject>
{
    public static abstract string Serialize(TValueObject valueObject);
    public static abstract TValueObject Deserialize(string rawValue);
}
