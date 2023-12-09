namespace FlipsideCrypto.NET.Configuration;
public class FlipsideOptionBuilder
{
    public string? ApiKey { get; set; }
    public Uri? BaseUri { get; set; } = new Uri("https://api-v2.flipsidecrypto.xyz");

    internal FlipsideOptions Build()
    {
        if(String.IsNullOrWhiteSpace(ApiKey))
        {
            throw new InvalidOperationException("No APIKey set");
        }
        if(BaseUri is null || (BaseUri.Scheme != "http" && BaseUri.Scheme != "https"))
        {
            throw new InvalidOperationException("Invalid BaseUri. Must be a http or https url");
        }
        //
        return new FlipsideOptions(ApiKey, BaseUri);
    }
}
