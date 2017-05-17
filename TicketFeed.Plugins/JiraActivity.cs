using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using TicketFeed.SDK;

namespace TicketFeed.Plugins
{
	internal sealed class JiraActivity : Source
	{
		private const int MaxRecordsToPull = 100;

		private readonly string url;
		private readonly string username;
		private readonly string password;

		public JiraActivity(string url, string username, string password)
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
				string credentials = Convert.ToBase64String(Encoding.Default.GetBytes(this.username + ":" + this.password));
				client.Headers[HttpRequestHeader.Authorization] = $"Basic {credentials}";

				string feedUrl =
					$"{this.url}/activity?streams=user+IS+{this.username}&" +
					$"streams=update-date+BETWEEN+{dateRange.Start.UnixTime()}+{dateRange.End.UnixTime()}&" +
					$"maxResults={MaxRecordsToPull}&" +
					"os_authType=basic";
#if DEBUG
				Console.WriteLine(dateRange.ToString());
				Console.WriteLine(feedUrl);
#endif
				string response = client.DownloadString(feedUrl);
				Console.WriteLine($"r: {response}");

				var xDocument = XDocument.Parse(response);
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