using System;
using System.Linq;
using System.Threading.Tasks;

namespace PhishReport
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
            PhishingTakedown takedown2 = await phish.GetTakedown("case_4kr52xX9zZA");
            Console.WriteLine($"ID: {takedown2.Id}");
            Console.WriteLine($"URL: {takedown2.Url}");

            Console.WriteLine($"> Getting the latest Indicator of Kit (IoK) matches");
            IoKMatch[] matches = await phish.GetMatches();
            Console.WriteLine($"Received {matches.Length} matches from the following indicators: {string.Join(", ", matches.Select(x => x.IndicatorId).Distinct())}");

            Console.WriteLine("Demo finished");
            Console.ReadKey();
        }
    }
}