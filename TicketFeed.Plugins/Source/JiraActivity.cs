using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
            private const string ConfigFilePath = "Jira.json";

            // ReSharper disable UnusedAutoPropertyAccessor.Local : properties are set upon deserialization from JSON
            // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local : properties are set upon deserialization from JSON
            public int MaxRecordsToPull { get; set; } = 300;
            public string Url { get; set; }
            public string Username { get; set; }

            // ReSharper disable once MemberCanBePrivate.Local : : properties are set upon deserialization from JSON
            public string Password { get; set; }

            private Config() { }

            public static Config Read()
            {
                if (!System.IO.File.Exists(ConfigFilePath))
                    throw new InvalidConfigurationException($"No settings file found at {ConfigFilePath}");
                string settings = System.IO.File.ReadAllText(ConfigFilePath);
                try
                {
                    return JsonConvert.DeserializeObject<Config>(settings);
                }
                catch
                {
                    throw new InvalidConfigurationException($"Improperly formatted settings file {ConfigFilePath}");
                }
            }

            public NetworkCredential Credential()
            {
                return new NetworkCredential(Username, Password);
            }

            public string CredentialBase64()
            {
                return Convert.ToBase64String(Encoding.Default.GetBytes(Username + ":" + Password));
            }

            public override string ToString()
            {
                return $"{nameof(MaxRecordsToPull)}: {MaxRecordsToPull}, {nameof(Url)}: {Url}, {nameof(Username)}: {Username}, {nameof(Password)}: {Password}";
            }
        }

        private readonly Config config;

        public JiraActivity()
        {
            this.config = Config.Read();
        }

        public override string Name => "Jira";

        public override Tickets Tickets(DateRange dateRange)
        {
            using (var client = new WebClient { Credentials = this.config.Credential() })
            {
                string credentials = this.config.CredentialBase64();
                client.Headers[HttpRequestHeader.Authorization] = $"Basic {credentials}";

                string feedUrl =
                    $"{this.config.Url}/activity?streams=user+IS+{this.config.Username}&" +
                    $"streams=update-date+BETWEEN+{dateRange.Start.UnixTime()}+{dateRange.End.UnixTime()}&" +
                    $"maxResults={this.config.MaxRecordsToPull}&" +
                    "os_authType=basic";
#if DEBUG
                Console.WriteLine(dateRange.ToString());
                Console.WriteLine(feedUrl);
                Console.WriteLine(this.config.ToString());
#endif
                string response = client.DownloadString(feedUrl);
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
            if (!xmlDocument.HasElements)
            {
                var element = new XElement(xmlDocument.Name.LocalName) { Value = xmlDocument.Value };
                foreach (XAttribute attribute in xmlDocument.Attributes())
                    element.Add(attribute);
                return element;
            }

            return new XElement(xmlDocument.Name.LocalName,
                xmlDocument.Elements().Select(ClearNamespaces));
        }
    }
}