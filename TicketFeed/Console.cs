using TicketFeed.SDK;

namespace TicketFeed
{
    internal class Console : IOutput
    {
        public string Name => "Console";

        public void Print(FeedRecords records)
        {
            System.Console.Write(records.ToString());
        }
    }
}