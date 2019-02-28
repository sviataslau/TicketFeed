using TicketFeed.SDK;

namespace TicketFeed
{
    // ReSharper disable once UnusedMember.Global : instantiated dynamically by host
    internal sealed class Console : Output
    {
        public override string Name => "Console";

        public override void Print(Tickets records)
        {
            System.Console.Write(records.ToString());
        }
    }
}