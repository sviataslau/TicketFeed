using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

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
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Working on pulling data from Jira. Please, be patient.");
            Console.ResetColor();
            var feed = new TicketFeed(options.Url, options.User, options.Password);
            FeedRecords records = feed.Records(new DateRange(options.Range));
            string output = records.ToString();
            switch (options.Output)
            {
                case Output.Console:
                    Console.Write(output);
                    break;
                case Output.File:
                    Console.Write(output);
                    File.WriteAllBytes("Feed.txt", Encoding.UTF8.GetBytes(output));
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("The result has been copied to Feed.txt");
                    Console.ResetColor();
                    break;
                case Output.Clipboard:
                    Console.Write(output);
                    Clipboard.SetText(output);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("The result has been copied to clipboard");
                    Console.ResetColor();
                    break;
                default:
                    Console.Write("No output specified");
                    break;
            }
        }
    }
}