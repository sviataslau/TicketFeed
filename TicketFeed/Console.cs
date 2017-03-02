using TicketFeed.SDK;

namespace TicketFeed
{
    internal class Console : Output
    {
        public override string Name => "Console";

        public override void Print(Tickets records)
        {
            System.Console.Write(records.ToString());
        }
    }
}