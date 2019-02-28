using System;
using System.Collections.Generic;
using System.Linq;
using TicketFeed.SDK;

namespace TicketFeed.Plugins.Source
{
    // ReSharper disable once UnusedMember.Global : instantiated dynamically by host
    internal sealed class Excuse : SDK.Source
    {
        private class Config
        {
            private const string ConfigFilePath = "Excuse.txt";

            private readonly Random seed = new Random();
            private readonly IReadOnlyCollection<string> phrases;

            private Config(IReadOnlyCollection<string> phrases)
            {
                this.phrases = phrases;
            }

            public static Config Read()
            {
                if (!System.IO.File.Exists(ConfigFilePath))
                    throw new InvalidConfigurationException($"No settings file found at {ConfigFilePath}");
                string[] phrases = System.IO.File.ReadLines(ConfigFilePath).ToArray();
                if (!phrases.Any())
                    throw new InvalidConfigurationException($"No phrases found in {ConfigFilePath}");
                return new Config(phrases);
            }

            public string RandomExcuse()
            {
                return this.phrases.ElementAt(this.seed.Next(this.phrases.Count));
            }
        }

        private readonly Config config;

        public Excuse()
        {
            this.config = Config.Read();
        }

        public override string Name => "Excuse";

        public override Tickets Tickets(DateRange dateRange)
        {
            var tickets = new Tickets();
            foreach (DateTime day in dateRange.Days())
            {
                tickets.Add(day, this.config.RandomExcuse());
            }

            return tickets;
        }
    }
}