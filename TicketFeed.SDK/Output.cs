namespace TicketFeed.SDK
{
    public abstract class Output : ILoadableItem
    {
        public abstract string Name { get; }
        public abstract void Print(Tickets records);
    }
}