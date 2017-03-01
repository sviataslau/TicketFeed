using System;
using System.Collections.Generic;
using System.Text;

namespace TicketFeed
{
    internal class FeedRecords : Dictionary<DateTime, string>
    {
        private readonly DateRange dateRange;

        public FeedRecords(DateRange dateRange)
        {
            this.dateRange = dateRange;
        }

        public override string ToString()
        {
            bool oneDayRange = this.dateRange.Time().Days < 1;
            var builder = new StringBuilder();
            foreach (var record in this)
            {
                if (oneDayRange)
                {
                    builder.AppendLine(record.Value);
                }
                else
                {
                    builder.AppendLine(record.Key.ToShortDateString());
                    builder.Append(record.Value);
                }
                builder.AppendLine();
            }
            return builder.ToString();
        }
    }
}