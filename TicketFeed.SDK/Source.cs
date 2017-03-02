namespace TicketFeed.SDK
{
    public abstract class Source : ILoadableItem
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly string url;
        // ReSharper disable once NotAccessedField.Local
        private readonly string username;
        // ReSharper disable once NotAccessedField.Local
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