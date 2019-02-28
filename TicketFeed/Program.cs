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
            if (output == null)
            {
                System.Console.ForegroundColor = ConsoleColor.DarkRed;
                System.Console.WriteLine("Invalid Output");
                System.Console.ResetColor();
            }
            else
            {
                Source source = Factory.Source(options.Source, options.Url, options.User, options.Password);
                if (source == null)
                {
                    System.Console.ForegroundColor = ConsoleColor.DarkRed;
                    System.Console.WriteLine("Invalid Source");
                    System.Console.ResetColor();
                }
                else
                {
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                    System.Console.WriteLine($"Working on pulling data from {source.Name}. Please, be patient.");
                    System.Console.ResetColor();
                    try
                    {
                        Tickets records = source.Tickets(new DateRange(options.Range));
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

            System.Console.ReadLine();
        }
    }
}