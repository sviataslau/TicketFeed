using TicketFeed.SDK;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TicketFeed.Plugins
{
    // ReSharper disable once UnusedMember.Global : instantiated dynamically by host
    internal class Tacc : Output
    {
        // ReSharper disable once ClassNeverInstantiated.Local : instantiated with JSON deserialization
        private class Config
        {
            private const string ConfigFilePath = "Tacc.json";

            // ReSharper disable UnusedAutoPropertyAccessor.Local : properties are set upon deserialization from JSON
            public int EmployeeId { get; set; }
            public int ContractId { get; set; }
            public int WorkingTime { get; set; }
            public string Cookie { get; set; }
            public string Url { get; set; }

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
        }

        private readonly Config config;

        public Tacc()
        {
            this.config = Config.Read();
        }

        public override string Name => "TACC";

        public override void Print(Tickets records)
        {
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
                        new KeyValuePair<string, string>("ContractId", this.config.ContractId.ToString()),
                        new KeyValuePair<string, string>("EmployeeId", this.config.EmployeeId.ToString()),
                        new KeyValuePair<string, string>("Date", date),
                        new KeyValuePair<string, string>("Description", description),
                        new KeyValuePair<string, string>("TotalHourId", this.config.WorkingTime.ToString())
                    });
                    var request = new HttpRequestMessage(HttpMethod.Post, this.config.Url);
                    request.Headers.Add("Cookie", this.config.Cookie);
                    request.Content = formContent;
                    Task<HttpResponseMessage> task = client.SendAsync(request);
                    HttpResponseMessage result = task.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"The result for {date} has been sent to TACC");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"The result for {date} has NOT been sent to TACC");
                        Console.WriteLine(result.ReasonPhrase);
                    }

                    Console.ResetColor();
                }
            }
        }
    }
}