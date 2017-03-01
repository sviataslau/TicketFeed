using System.Text;
using CommandLine;

namespace TicketFeed
{
    internal enum Output
    {
        Console,
        File,
        Clipboard
    }

    internal class Options
    {
        [Option('j', "jira", Required = true, HelpText = "An URL for your Jira")]
        public string Url { get; set; }

        [Option('u', "user", Required = true, HelpText = "Jira username")]
        public string User { get; set; }

        [Option('p', "password", Required = true, HelpText = "Jira password")]
        public string Password { get; set; }

        [Option("range", Required = true, DefaultValue = DateRange.Type.Today,
            HelpText = "Date range which you want to pull")]
        public DateRange.Type Range { get; set; }

        [Option('o', "out", Required = true, DefaultValue = Output.File, HelpText = "Output")]
        public Output Output { get; set; }

        [HelpOption]
        public string Help()
        {
            var usage = new StringBuilder();
            usage.AppendLine("Ticket Feed 1.0");
            usage.AppendLine(
                "A simple console application to pull a list of tickets you've been working on from your JIRA.");
            usage.AppendLine(
                "TicketFeed.exe --jira https://comrex.atlassian.net --user you --password password --range Today[Yesterday|Week|Month] --out File[Console|Clipboard]");
            return usage.ToString();
        }
    }
}