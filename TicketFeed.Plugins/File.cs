using System;
using System.Text;
using TicketFeed.SDK;

namespace TicketFeed.Plugins
{
    // ReSharper disable once UnusedMember.Global : instantiated dynamically by host
    internal class File : Output
    {
        private const string Path = "Feed.txt";

        public override string Name => "File";

        public override void Print(Tickets records)
        {
            string output = records.ToString();
            Console.Write(output);
            System.IO.File.WriteAllBytes(Path, Encoding.UTF8.GetBytes(output));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"The result has been copied to {Path}");
            Console.ResetColor();
        }
    }
}