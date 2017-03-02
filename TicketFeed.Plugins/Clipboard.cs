using System;
using TicketFeed.SDK;

namespace TicketFeed.Plugins
{
    internal class Clipboard : IOutput
    {
        public string Name => "Clipboard";

        public void Print(FeedRecords records)
        {
            string output = records.ToString();
            Console.Write(output);
            System.Windows.Forms.Clipboard.SetText(output);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("The result has been copied to clipboard");
            Console.ResetColor();
        }
    }
}