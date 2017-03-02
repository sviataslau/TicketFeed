using System;
using System.Text;
using TicketFeed.SDK;

namespace TicketFeed.Plugins
{
    internal class File : IOutput
    {
        private const string Path = "Feed.txt";
        public string Name => "File";

        public void Print(FeedRecords records)
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