using System;
using System.IO;
using System.Linq;
using TicketFeed.SDK;

namespace TicketFeed
{
    internal class Factory
    {
        private const string PluginsDirectory = "Plugins";

        static Factory()
        {
            LoadPlugins();
        }

        private static void LoadPlugins()
        {
            if (Directory.Exists(PluginsDirectory))
            {
                foreach (var file in Directory.GetFiles(PluginsDirectory).Where(f => f.Contains(".dll")))
                    AppDomain.CurrentDomain.Load(File.ReadAllBytes(file));
            }
        }

        public static Output Output(string type)
        {
            return CreatePluginItem<Output>(type);
        }

        public static Source Source(string type, string url, string username, string password)
        {
            return CreatePluginItem<Source>(type, url, username, password);
        }

        private static T CreatePluginItem<T>(string type, params object[] parameters)
            where T : ILoadableItem
        {
            Type baseType = typeof(T);
            T[] outputs = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => baseType.IsAssignableFrom(p) && !p.IsAbstract)
                .Select(p => Activator.CreateInstance(p, parameters))
                .Cast<T>()
                .ToArray();
            return outputs.FirstOrDefault(o => o.Name.Trim().Equals(type, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}