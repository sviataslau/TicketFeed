using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Newtonsoft.Json;
using TicketFeed.SDK;

namespace TicketFeed.Plugins.Source
{
    // ReSharper disable once UnusedMember.Global : instantiated dynamically by host
    internal sealed class JiraActivity : SDK.Source
    {
        // ReSharper disable once ClassNeverInstantiated.Local : instantiated with JSON deserialization
        private class Config
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local : properties are set upon deserialization from JSON
            // ReSharper disable AutoPropertyCanBeMadeGetOnly.Local : properties are set upon deserialization from JSON
            // ReSharper disable MemberCanBePrivate.Local : properties are set upon deserialization from JSON
            public int MaxRecordsToPull { get; set; } = 300;
            public string Url { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Token { get; set; }

            private Config() { }

            public static Config FromFile(string path)
            {
                if (!System.IO.File.Exists(path))
                    throw new InvalidConfigurationException($"No settings file found at {path}");
                string settings = System.IO.File.ReadAllText(path);
                try
                {
                    return JsonConvert.DeserializeObject<Config>(settings);
                }
                catch
                {
                    throw new InvalidConfigurationException($"Improperly formatted settings file {path}");
                }
            }

            public NetworkCredential Credential() => new NetworkCredential(Email, Token);

            public string FeedUrl(DateRange range) =>
                $"{Url}/activity?streams=user+IS+{Username}&" +
                $"streams=update-date+BETWEEN+{range.Start.UnixTime()}+{range.End.UnixTime()}&" +
                $"maxResults={MaxRecordsToPull}&" +
                "os_authType=basic";


            public override string ToString() =>
                $"{nameof(MaxRecordsToPull)}: {MaxRecordsToPull}, {nameof(Url)}: {Url}, {nameof(Username)}: {Username}, {nameof(Token)}: {Token}";
        }

        public override string Name => "Jira";

        public override Tickets Tickets(DateRange dateRange)
        {
            Config config = Config.FromFile("Jira.json");
            using (var client = new WebClient { Credentials = config.Credential() })
            {
                string feedUrl = config.FeedUrl(dateRange);
#if DEBUG
                Console.WriteLine(dateRange.ToString());
                Console.WriteLine(feedUrl);
                Console.WriteLine(config.ToString());
#endif
                string response = client.DownloadString(feedUrl);
                Console.WriteLine(response);
                XDocument xDocument = XDocument.Parse(response);
                XElement feed = ClearNamespaces(xDocument.Root);
                XElement[] entries = feed.Descendants("entry").ToArray();
                IEnumerable<Tuple<DateTime, string>> tickets = from entry in entries
                    let updated = DateTime.Parse(entry.Element("updated")?.Value)
                    let target = entry.Element("target") ?? entry.Element("object")
                    let title = target?.Element("title")
                    let summary = target?.Element("summary")
                    let type = target?.Element("object-type")
                    where type != null && type.Value.Contains("issue")
                    select new Tuple<DateTime, string>(updated, title?.Value + " " + summary?.Value);
                IDictionary<DateTime, string> result = tickets.GroupBy(t => t.Item1.Date, t => t.Item2)
                    .OrderByDescending(t => t.Key)
                    .Where(t => dateRange.Contains(t.Key))
                    .ToDictionary(g => g.Key, g => string.Join(Environment.NewLine, g.Distinct().ToArray()));
                var fr = new Tickets();
                foreach (KeyValuePair<DateTime, string> record in result)
                    fr.Add(record.Key, record.Value);
                return fr;
            }
        }

        private static XElement ClearNamespaces(XElement xmlDocument)
        {
            if (xmlDocument.HasElements)
                return new XElement(xmlDocument.Name.LocalName,
                    xmlDocument.Elements().Select(ClearNamespaces));
            var element = new XElement(xmlDocument.Name.LocalName) { Value = xmlDocument.Value };
            foreach (XAttribute attribute in xmlDocument.Attributes())
                element.Add(attribute);
            return element;
        }
    }
}