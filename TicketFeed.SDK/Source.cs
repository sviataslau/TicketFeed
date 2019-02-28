namespace TicketFeed.SDK
{
    public abstract class Source : ILoadableItem
    {
        public abstract string Name { get; }
        public abstract Tickets Tickets(DateRange dateRange);
    }
}