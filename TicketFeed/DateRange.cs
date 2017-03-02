using System;

namespace TicketFeed
{
    internal class DateRange
    {
        internal enum Type
        {
            Today,
            Yesterday,
            Week,
            Month
        }

        public readonly DateTime Start;
        public readonly DateTime End;

        internal DateRange(Type type)
        {
            switch (type)
            {
                case Type.Today:
                    this.Start = DateTime.Today;
                    this.End = RoundToEndOfDay(this.Start);
                    break;
                case Type.Yesterday:
                    this.Start = DateTime.Today.AddDays(-1);
                    this.End = RoundToEndOfDay(this.Start);
                    break;
                case Type.Week:
                    this.Start = StartOfWeek(DateTime.Now.Date, DayOfWeek.Monday);
                    this.End = RoundToEndOfDay(this.Start.AddDays(7));
                    break;
                case Type.Month:
                    this.Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    this.End = this.Start.AddMonths(1).AddDays(-1);
                    break;
            }
        }

        private static DateTime RoundToEndOfDay(DateTime date)
        {
            return date.Date.AddDays(1).AddSeconds(-1);
        }

        private static DateTime StartOfWeek(DateTime date, DayOfWeek startOfWeek)
        {
            int diff = date.DayOfWeek - startOfWeek;
            if (diff < 0)
                diff += 7;
            return date.AddDays(-1 * diff).Date;
        }

        public bool Contains(DateTime date)
        {
            return date >= this.Start && date <= this.End;
        }

        public TimeSpan Time()
        {
            return this.End.Subtract(this.Start);
        }
    }
}