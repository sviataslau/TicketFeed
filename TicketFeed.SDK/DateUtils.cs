using System;

namespace TicketFeed.SDK
{
    public static class DateUtils
    {
        public static long UnixTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalMilliseconds);
        }

        public static DateTime EndOfDay(this DateTime date) => date.Date.AddDays(1).AddSeconds(-1);

        public static DateTime StartOfWeek(this DateTime date, DayOfWeek startOfWeek)
        {
            int diff = date.DayOfWeek - startOfWeek;
            if (diff < 0)
                diff += 7;
            return date.AddDays(-1 * diff).Date;
        }

        public static bool IsWeekend(this DateTime date) =>
            date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }
}