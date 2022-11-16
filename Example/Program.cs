using System;
using System.Linq;
using System.Threading.Tasks;
using PhishReport;

namespace Example
{
    public static class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("Enter the Phish.Report API Key:");
            Console.WriteLine("Instructions on how to obtain this are in the Github repository.");

            string key = Console.ReadLine();

            Console.WriteLine();

            PhishReportClient phish = new(key);

            Console.WriteLine($"> Starting a phishing takedown");
            PhishingTakedown takedown1 = await phish.CreateTakedown("https://seowqpeoqwakfd425.ml/dssdfds-fsdfsdf0s-df0ds0f0dsdfsdd0f0s-df0dfgdd8658/");
            Console.WriteLine($"ID: {takedown1.Id}");
            Console.WriteLine($"URL: {takedown1.Url}");
  
            Console.WriteLine($"> Getting a phishing takedown");
            PhishingTakedown takedown2 = await phish.GetTakedown("case_4ExZCRk3PAh");
            Console.WriteLine($"ID: {takedown2.Id}");
            Console.WriteLine($"URL: {takedown2.Url}");

            Console.WriteLine($"> Getting the latest Indicator of Kit (IoK) matches");
            IoKMatch[] matches = await phish.GetIoKMatches();
            Console.WriteLine($"Received {matches.Length} matches from the following indicators: {string.Join(", ", matches.Select(x => x.IndicatorId).Distinct())}");

            Console.WriteLine("> Polling for new Indicator of Kit (IoK) matches");
            phish.IoKMatched += (sender, match) =>
            {
                Console.WriteLine($"{match.IndicatorId} match on {match.Url}, source: https://urlscan.io/result/{match.UrlscanUUID}/");
            };

            Console.WriteLine();
            Console.WriteLine("Demo finished");
            Console.ReadKey();
        }
    }
}