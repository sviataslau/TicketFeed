namespace TicketFeed.SDK
{
    public interface IOutput
    {
        string Name { get; }
        void Print(FeedRecords records);
    }
}