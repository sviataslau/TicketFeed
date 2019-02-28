using TicketFeed.SDK;

namespace TicketFeed
{
    public class NoOutput : Output
    {
        public override string Name => "Null";
        public override void Print(Tickets records) { }
    }
}