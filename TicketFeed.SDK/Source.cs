namespace TicketFeed.SDK
{
    // ReSharper disable NotAccessedField.Local
    public abstract class Source : ILoadableItem
    {
        private readonly string url;
        private readonly string username;
        private readonly string password;

        protected Source(string url, string username, string password)
        {
            this.url = url;
            this.username = username;
            this.password = password;
        }

        public abstract string Name { get; }
        public abstract Tickets Tickets(DateRange dateRange);
    }
}