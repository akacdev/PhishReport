# Phish.Report 🎣

![](https://raw.githubusercontent.com/actually-akac/PhishReport/master/PhishReport/icon.png)

### An async C# library for interacting with the Phish.Report, Indicator of Kit and detection beta APIs.

**Warning**

> The Phish.Report API is still under development, so in the event that you start experiencing unexpected errors, first check if there's an update available or raise an issue in this repository.

## Usage
This library provides an easy interface for interacting with the Phish.Report APIs. You can create and track phishing takedowns and work with the [Indicator of Kit (IOK)](https://phish.report/IOK) platform.

API keys are required to use the API. Create one at: https://phish.report/user

To get started, import the library into your solution with either the `NuGet Package Manager` or the `dotnet` CLI.
```rust
dotnet add package PhishReport
```

For the primary classes to become available, import the used namespace.
```csharp
using PhishReport;
```

Need more examples? Under the `Example` directory you can find a working demo project that implements this library.

## Properties
- Built for **.NET 8**, **.NET 7** and **.NET 6**
- Fully **async**
- Coverage of the current **beta API**
- Extensive **XML documentation**
- **No external dependencies** (makes use of built-in `HttpClient` and `JsonSerializer`)
- **Custom exceptions** (`PhishReportException`) for easy debugging

## Features
- Create phishing takedowns
- Fetch existing reported cases
- Process newly created [Indicator of Kit](https://phish.report/IOK/) matches

## Code Samples

### Initializing a new API client
```csharp
PhishReportClient phish = new("API KEY");
```

### Creating a new phishing takedown
```csharp
PhishingTakedown takedown1 = await phish.CreateTakedown("https://alpsautorepairv.ml/?gclid=EAIaIQobChMIsfmc__Ds-wIVSOHICh3oGwtsEAAYASAAEgIxmPD_BwE");
```

### Retrieving an existing phishing takedown by its ID
```csharp
PhishingTakedown takedown2 = await phish.GetTakedown("case_4ExZCRk3PAh");
```

### Retrieving the latest [Indicator of Kit](https://phish.report/IOK/) matches
```csharp
IokMatch[] matches = await phish.GetIokMatches();
```

### Processing [Indicator of Kit](https://phish.report/IOK/) matches in real time
```csharp
phish.IokMatched += (sender, match) =>
{
	Console.WriteLine($"{match.IndicatorId} match on {match.Url}, source: https://urlscan.io/result/{match.UrlscanUUID}/");
};
```

## Resources
- Website: https://phish.report
- Documentation: https://phish.report/docs
- Indicator of Kit: https://phish.report/IOK/, https://github.com/phish-report/IOK

*This is a community-ran library. Not affiliated with Phish Report Ltd.*