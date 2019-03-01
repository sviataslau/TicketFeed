using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TicketFeed.SDK;

namespace TicketFeed.Plugins.Output
{
    // ReSharper disable once UnusedMember.Global : instantiated dynamically by host
    internal sealed class Tacc : SDK.Output
    {
        // ReSharper disable once ClassNeverInstantiated.Local : instantiated with JSON deserialization
        private class Config
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local : properties are set upon deserialization from JSON
            public int EmployeeId { get; set; }
            public int ContractId { get; set; }
            public int WorkingTime { get; set; }
            public string Cookie { get; set; }
            public string Url { get; set; }

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
        }

        public override string Name => "TACC";

        public override void Print(Tickets records)
        {
            Config config = Config.FromFile("Tacc.json");
            string output = records.ToString();
            Console.Write(output);
            using (var handler = new HttpClientHandler
            {
                UseCookies = false
            })
            using (var client = new HttpClient(handler))
            {
                foreach (KeyValuePair<DateTime, string> day in records)
                {
                    string date = day.Key.ToString("MM/dd/yyyy");
                    string description = day.Value;
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("ContractId", config.ContractId.ToString()),
                        new KeyValuePair<string, string>("EmployeeId", config.EmployeeId.ToString()),
                        new KeyValuePair<string, string>("Date", date),
                        new KeyValuePair<string, string>("Description", description),
                        new KeyValuePair<string, string>("TotalHourId", config.WorkingTime.ToString())
                    });
                    var request = new HttpRequestMessage(HttpMethod.Post, config.Url);
                    request.Headers.Add("Cookie", config.Cookie);
                    request.Content = formContent;
                    Task<HttpResponseMessage> task = client.SendAsync(request);
                    HttpResponseMessage result = task.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        using (new WithConsoleColor(ConsoleColor.Green))
                        {
                            Console.WriteLine($"The result for {date} has been sent to TACC");
                        }
                    }
                    else
                    {
                        using (new WithConsoleColor(ConsoleColor.DarkRed))
                        {
                            Console.WriteLine($"The result for {date} has NOT been sent to TACC");
                            Console.WriteLine(result.ReasonPhrase);
                        }
                    }
                }
            }
        }
    }
}