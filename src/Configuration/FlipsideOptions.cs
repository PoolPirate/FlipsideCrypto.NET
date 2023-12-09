namespace FlipsideCrypto.NET.Configuration;
public class FlipsideOptions
{
    public string ApiKey { get; }
    public Uri BaseUri { get; }

    internal FlipsideOptions(string apiKey, Uri baseUri)
    {
        ApiKey = apiKey;
        BaseUri = baseUri;
    }
}
