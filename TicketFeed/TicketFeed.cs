using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace TicketFeed
{
    internal sealed class TicketFeed
    {
        private const int RecordsToPull = 100;
        private readonly string jiraUrl;
        private readonly string username;
        private readonly string password;

        public TicketFeed(string jiraUrl, string username, string password)
        {
            this.jiraUrl = jiraUrl;
            this.username = username;
            this.password = password;
        }

        public FeedRecords Records(DateRange dateRange)
        {
            using (var client = new WebClient { Credentials = new NetworkCredential(this.username, this.password) })
            {
                bool oneDayRange = dateRange.Time().Days < 1;
                int recordsToPull = oneDayRange ? 20 : RecordsToPull;
                string url =
                    $"{this.jiraUrl}/activity?maxResults={recordsToPull}&streams=user+IS+{this.username}&os_authType=basic";
                string response = client.DownloadString(url);
                XElement feed = ClearNamespaces(XDocument.Parse(response).Root);
                XElement[] entries = feed.Descendants("entry").ToArray();
                IEnumerable<Tuple<DateTime, string>> targetTickets = from entry in entries
                    let updated = DateTime.Parse(entry.Element("updated")?.Value)
                    let target = entry.Element("target")
                    let title = target?.Element("title")
                    let summary = target?.Element("summary")
                    let type = target?.Element("object-type")
                    where type != null && type.Value.Contains("issue")
                    select new Tuple<DateTime, string>(updated, title?.Value + " " + summary?.Value);
                IEnumerable<Tuple<DateTime, string>> objectTickets = from entry in entries
                    let updated = DateTime.Parse(entry.Element("updated")?.Value)
                    let target = entry.Element("object")
                    let title = target?.Element("title")
                    let summary = target?.Element("summary")
                    let type = target?.Element("object-type")
                    where type != null && type.Value.Contains("issue")
                    select new Tuple<DateTime, string>(updated, title?.Value + " " + summary?.Value);
                IEnumerable<Tuple<DateTime, string>> tickets = targetTickets.Concat(objectTickets).ToArray();
                IDictionary<DateTime, string> result = tickets.GroupBy(t => t.Item1.Date, t => t.Item2)
                    .Where(t => dateRange.IsInRange(t.Key.Date))
                    .OrderByDescending(t => t.Key)
                    .ToDictionary(g => g.Key, g => string.Join(Environment.NewLine, g.Distinct().ToArray()));
                var fr = new FeedRecords(dateRange);
                foreach (var record in result)
                    fr.Add(record.Key, record.Value);
                return fr;
            }
        }

        private static XElement ClearNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                XElement element = new XElement(xmlDocument.Name.LocalName) { Value = xmlDocument.Value };
                foreach (XAttribute attribute in xmlDocument.Attributes())
                    element.Add(attribute);
                return element;
            }
            return new XElement(xmlDocument.Name.LocalName,
                xmlDocument.Elements().Select(ClearNamespaces));
        }
    }
}