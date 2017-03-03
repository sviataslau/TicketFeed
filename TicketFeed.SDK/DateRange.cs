using System;
using System.Globalization;

namespace TicketFeed.SDK
{
    public class DateRange
    {
        public enum Type
        {
            Today,
            Yesterday,
            Week,
            Month
        }

        public readonly DateTime Start;
        public readonly DateTime End;

        public DateRange(Type type)
        {
            switch (type)
            {
                case Type.Today:
                    this.Start = DateTime.UtcNow.Date;
                    this.End = this.Start.EndOfDay();
                    break;
                case Type.Yesterday:
                    this.Start = DateTime.UtcNow.Date.AddDays(-1);
                    this.End = this.Start.EndOfDay();
                    break;
                case Type.Week:
                    this.Start = DateTime.UtcNow.Date.StartOfWeek(DayOfWeek.Monday);
                    this.End = this.Start.AddDays(7).EndOfDay();
                    break;
                case Type.Month:
                    this.Start = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                    this.End = this.Start.AddMonths(1).AddDays(-1);
                    break;
            }
        }

        public bool Contains(DateTime dateTime)
        {
            return dateTime >= this.Start && dateTime < this.End;
        }

        public TimeSpan Time()
        {
            return this.End.Subtract(this.Start);
        }

        public override string ToString()
        {
            return $"{this.Start.ToString(CultureInfo.InvariantCulture)} ({this.Start.UnixTime()})" +
                   $" {this.End.ToString(CultureInfo.InvariantCulture)} ({this.End.UnixTime()})";
        }
    }
}