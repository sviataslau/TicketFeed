using System;
using System.IO;
using System.Linq;
using TicketFeed.SDK;

namespace TicketFeed
{
    internal class Output
    {
        private const string PluginsDirectory = "Plugins";

        public static IOutput Create(string type)
        {
            LoadPlugins();
            return LoadOutput(type);
        }

        private static void LoadPlugins()
        {
            if (Directory.Exists(PluginsDirectory))
            {
                foreach (var file in Directory.GetFiles(PluginsDirectory).Where(f => f.Contains(".dll")))
                    AppDomain.CurrentDomain.Load(File.ReadAllBytes(file));
            }
        }

        private static IOutput LoadOutput(string type)
        {
            Type @interface = typeof(IOutput);
            IOutput[] outputs = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => @interface.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IOutput>()
                .ToArray();
            return outputs.FirstOrDefault(o => o.Name.Trim().Equals(type));
        }
    }
}