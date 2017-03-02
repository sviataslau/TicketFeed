using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using TicketFeed.SDK;

namespace TicketFeed.Plugins
{
    internal sealed class JiraFeed : Source
    {
        private const int RecordsToPull = 100;
        private readonly string url;
        private readonly string username;
        private readonly string password;

        public JiraFeed(string url, string username, string password)
            : base(url, username, password)
        {
            this.url = url;
            this.username = username;
            this.password = password;
        }

        public override string Name => "Jira";

        public override Tickets Tickets(DateRange dateRange)
        {
            using (var client = new WebClient {Credentials = new NetworkCredential(this.username, this.password)})
            {
                bool oneDayRange = dateRange.Time().Days < 1;
                int recordsToPull = oneDayRange ? 20 : RecordsToPull;
                string url =
                    $"{this.url}/activity?maxResults={recordsToPull}&streams=user+IS+{this.username}&os_authType=basic";
                string response = client.DownloadString(url);
                XElement feed = ClearNamespaces(XDocument.Parse(response).Root);
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
                    .Where(t => dateRange.Contains(t.Key.Date))
                    .OrderByDescending(t => t.Key)
                    .ToDictionary(g => g.Key, g => string.Join(Environment.NewLine, g.Distinct().ToArray()));
                var fr = new Tickets();
                foreach (var record in result)
                    fr.Add(record.Key, record.Value);
                return fr;
            }
        }

        private static XElement ClearNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                var element = new XElement(xmlDocument.Name.LocalName) {Value = xmlDocument.Value};
                foreach (XAttribute attribute in xmlDocument.Attributes())
                    element.Add(attribute);
                return element;
            }
            return new XElement(xmlDocument.Name.LocalName,
                xmlDocument.Elements().Select(ClearNamespaces));
        }
    }
}