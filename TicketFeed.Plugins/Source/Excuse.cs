using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TicketFeed.SDK;

namespace TicketFeed.Plugins.Source
{
    // ReSharper disable once UnusedMember.Global : instantiated dynamically by host
    internal sealed class Excuse : SDK.Source
    {
        private class Excuses : IEnumerable<string>
        {
            private readonly Random seed = new Random();
            private readonly IReadOnlyCollection<string> phrases;

            private Excuses(IReadOnlyCollection<string> phrases)
            {
                this.phrases = phrases;
            }

            public static Excuses FromFile(string path)
            {
                if (!System.IO.File.Exists(path))
                    throw new InvalidConfigurationException($"No settings file found at {path}");
                string[] phrases = System.IO.File.ReadLines(path).ToArray();
                if (!phrases.Any())
                    throw new InvalidConfigurationException($"No phrases found in {path}");
                return new Excuses(phrases);
            }

            public string RandomExcuse() => this.phrases.ElementAt(this.seed.Next(this.phrases.Count));

            public IEnumerator<string> GetEnumerator() => this.phrases.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public override string Name => "Excuse";

        public override Tickets Tickets(DateRange dateRange)
        {
            Excuses excuses = Excuses.FromFile("Excuse.txt");
            var tickets = new Tickets();
            IEnumerable<DateTime> weekDays = dateRange.Days().Where(d => !d.IsWeekend());
            foreach (DateTime day in weekDays)
            {
                tickets.Add(day, excuses.RandomExcuse());
            }

            return tickets;
        }
    }
}