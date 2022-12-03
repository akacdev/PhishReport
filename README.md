# Phish.Report 🎣

<div align="center">
  <img width="256" height="256" src="https://raw.githubusercontent.com/actually-akac/PhishReport/master/PhishReport/icon.png">
</div>

<div align="center">
  An async C# library for interacting with the Phish.Report, Indicator of Kit and detection beta APIs.
</div>

> **Warning**<br>
> The Phish.Report API is still under development, so in the event that you start experiencing unexpected errors, first check if there's an update available or raise an issue in this repository.

## Usage
Provides an easy interface for interacting with the Phish.Report APIs.

You can create and track phishing takedowns and work with the [Indicator of Kit (IOK)](https://phish.report/IOK) platform.

API keys are required to use the API. Create one at: https://phish.report/user

To get started, add the library into your solution with either the `NuGet Package Manager` or the `dotnet` CLI.
```rust
dotnet add package PhishReport
```

For the primary classes to become available, import the used namespace.
```csharp
using PhishReport;
```

Need more examples? Under the `Example` directory you can find a working demo project that implements this library.

## Features
- Built for **.NET 6** and **.NET 7**
- Fully **async**
- Coverage of the current **beta API**
- Extensive **XML documentation**
- **No external dependencies** (uses integrated HTTP and JSON)
- Create phishing takedowns, fetch existing cases or process **Indicator of Kit** matches.
- **Custom exceptions** (`PhishReportException`) for advanced catching
- Automatic request retries

## Code Samples

### Initializing a new API client
```csharp
PhishReportClient phish = new("API KEY");
```

### Creating a new phishing takedown
```csharp
PhishingTakedown takedown1 = await phish.CreateTakedown("https://seowqpeoqwakfd425.ml/dssdfds-fsdfsdf0s-df0ds0f0dsdfsdd0f0s-df0dfgdd8658/");
```

### Retrieving an existing phishing takedown by its ID
```csharp
PhishingTakedown takedown2 = await phish.GetTakedown("case_4tmKtcajnzj");
```

### Retrieving the latest [Indicator of Kit](https://phish.report/IOK/) matches
```csharp
IoKMatch[] matches = await phish.GetIoKMatches();
```
### Processing [Indicator of Kit](https://phish.report/IOK/) matches in real time
```csharp
phish.IoKMatched += (sender, match) =>
{
    Console.WriteLine($"{match.Url} just matched {match.IndicatorId}");
};
```

## Available Methods
- Task\<PhishingTakedown> **CreateTakedown**(string url)
- Task\<PhishingTakedown> **GetTakedown**(string id)
- Task\<IoKMatch[]> **GetIoKMatches**(int page = 0)

## Available Events
- EventHandler\<IoKMatch> IoKMatched

## Resources
- Website: https://phish.report
- Indicator of Kit: https://phish.report/IOK/, https://github.com/phish-report/IOK