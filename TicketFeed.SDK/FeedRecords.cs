using System;
using System.Collections.Generic;
using System.Text;

namespace TicketFeed.SDK
{
    public class FeedRecords : Dictionary<DateTime, string>
    {
        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var record in this)
            {
                builder.AppendLine(record.Key.ToShortDateString());
                builder.Append(record.Value);
                builder.AppendLine();
            }
            return builder.ToString();
        }
    }
}