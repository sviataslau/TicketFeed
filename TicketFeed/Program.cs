using System;
using TicketFeed.SDK;

namespace TicketFeed
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var options = new Options();
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
                return;
            IOutput output = Output.Create(options.Output);
            if (output == null)
            {
                System.Console.ForegroundColor = ConsoleColor.DarkRed;
                System.Console.WriteLine("Invalid Output");
                System.Console.ResetColor();
            }
            else
            {
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.WriteLine("Working on pulling data from Jira. Please, be patient.");
                System.Console.ResetColor();
                try
                {
                    var feed = new TicketFeed(options.Url, options.User, options.Password);
                    FeedRecords records = feed.Records(new DateRange(options.Range));
                    output.Print(records);
                }
                catch (Exception ex)
                {
                    System.Console.ForegroundColor = ConsoleColor.DarkRed;
                    System.Console.WriteLine(ex.Message);
                    System.Console.WriteLine(ex.StackTrace);
                }
            }
        }
    }
}