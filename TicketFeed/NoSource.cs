using TicketFeed.SDK;

namespace TicketFeed
{
    public class NoSource : Source
    {
        public override string Name => "Null";

        public override Tickets Tickets(DateRange dateRange)
        {
            return new Tickets();
        }
    }
}