using System;
using TicketFeed.SDK;

namespace TicketFeed
{
    // ReSharper disable once ClassNeverInstantiated.Global : instantiated by console host
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var options = new Options();
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
                return;
            Output output = Factory.Output(options.Output);
            Source source = Factory.Source(options.Source);

            using (new WithConsoleColor(ConsoleColor.Yellow))
            {
                System.Console.WriteLine($"Working on pulling data from {source.Name}. Please, be patient.");
            }

            try
            {
                Tickets records = source.Tickets(new DateRange(options.Range));
                using (new WithConsoleColor(ConsoleColor.Yellow))
                {
                    System.Console.WriteLine($"Sending data to {output.Name}. Please, be patient.");
                }

                output.Print(records);
            }
            catch (Exception ex)
            {
                using (new WithConsoleColor(ConsoleColor.DarkRed))
                {
                    System.Console.WriteLine(ex.Message);
                    System.Console.WriteLine(ex.StackTrace);
                }
            }

            System.Console.ReadLine();
        }
    }
}