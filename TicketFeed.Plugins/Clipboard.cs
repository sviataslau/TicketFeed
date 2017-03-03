using System;
using TicketFeed.SDK;

namespace TicketFeed.Plugins
{
    internal class Clipboard : Output
    {
        public override string Name => "Clipboard";

        public override void Print(Tickets records)
        {
            string output = records.ToString();
            Console.Write(output);
            if (!string.IsNullOrEmpty(output))
            {
                System.Windows.Forms.Clipboard.SetText(output);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("The result has been copied to clipboard");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("The result was empty");
            }
            Console.ResetColor();
        }
    }
}