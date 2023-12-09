# FlipsideCrypto.NET

A light-weight client for the [FlipsideCrypto SQL query api](https://flipsidecrypto.xyz/settings/api-keys). Now you have all the blockchain data on your fingertips with C#.

### Supports

- Creating / Cancelling Queries
- Retrieving their status
- Getting their result set & automatically mapping it to C# objects

## Usage

### Installation

- Install package from [Nuget](https://www.nuget.org/packages/FlipsideCrypto.NET/)
- Add it to your applications DI container.

```cs
services.AddFlipsideCrypto(API_KEY)
```

### Creating a query

```cs
IFlipsideClient fs = //get from DI

var runId = fs.CreateQueryRunAsync("SELECT max(block_number) AS maxblockheight FROM ethereum.core.fact_blocks");
```

### Getting query results

```cs
//Define class with property names matching query column names (case insensitive)
public class MaxBlockHeightQueryResult {
    public ulong MaxBlockHeight { get; init; }

    //Make sure to have a public parametereless constructor
    public MaxBlockHeightQueryResult() {}
}

MaxBlockHeightQueryResult[] results = await fs.GetQueryRunResults<MaxBlockHeightQueryResult>(YOUR_RUN_ID);
```
