using System;
using System.Collections.Generic;
using System.Globalization;

namespace TicketFeed.SDK
{
    public sealed class DateRange
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Type is not supported");
            }
        }

        public bool Contains(DateTime dateTime) => dateTime >= this.Start && dateTime < this.End;

        public IEnumerable<DateTime> Days()
        {
            DateTime current = this.Start;
            while (current <= this.End)
            {
                yield return current;
                current = current.AddDays(1);
            }
        }

        public override string ToString() =>
            $"{this.Start.ToString(CultureInfo.InvariantCulture)} ({this.Start.UnixTime()})" +
            $" {this.End.ToString(CultureInfo.InvariantCulture)} ({this.End.UnixTime()})";
    }
}