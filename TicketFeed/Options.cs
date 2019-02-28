using System.Text;
using CommandLine;
using TicketFeed.SDK;

namespace TicketFeed
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global : properties are set by CommandLineParser lib
    internal sealed class Options
    {
        [Option("src", Required = true, HelpText = "Source Type")]
        public string Source { get; set; }

        [Option("range", Required = true, DefaultValue = DateRange.Type.Today,
            HelpText = "Date range which you want to pull")]
        public DateRange.Type Range { get; set; }

        [Option('o', "out", Required = true, HelpText = "Output")]
        public string Output { get; set; }

        [HelpOption]
        // ReSharper disable once UnusedMember.Global : used by CommandLineParser lib
        public string Help()
        {
            var usage = new StringBuilder();
            usage.AppendLine("Ticket Feed 1.0");
            usage.AppendLine(
                "A simple console application to pull a list of tickets you've been working on from your bug tracking system and print the list to provided output.");
            usage.AppendLine(
                "TicketFeed.exe --src jira --range Today[Yesterday|Week|Month] --out file[console|clipboard|whatever]");
            return usage.ToString();
        }
    }
}