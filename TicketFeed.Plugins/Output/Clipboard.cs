using System;
using TicketFeed.SDK;

namespace TicketFeed.Plugins.Output
{
    // ReSharper disable once UnusedMember.Global : instantiated dynamically by host
    internal sealed class Clipboard : SDK.Output
    {
        public override string Name => "Clipboard";

        public override void Print(Tickets records)
        {
            string output = records.ToString();
            Console.Write(output);
            if (!string.IsNullOrEmpty(output))
            {
                System.Windows.Forms.Clipboard.SetText(output);
                using (new WithConsoleColor(ConsoleColor.Green))
                {
                    Console.WriteLine("The result has been copied to clipboard");
                }
            }
            else
            {
                using (new WithConsoleColor(ConsoleColor.DarkYellow))
                {
                    Console.WriteLine("The result was empty");
                }
            }
        }
    }
}