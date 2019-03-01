using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TicketFeed.SDK;

namespace TicketFeed
{
    internal static class Factory
    {
        private static readonly string PluginsDirectory =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        static Factory()
        {
            LoadPlugins();
        }

        private static void LoadPlugins()
        {
            using (new WithConsoleColor(ConsoleColor.Yellow))
            {
#if DEBUG
                System.Console.WriteLine("Loading plugins from {0}", PluginsDirectory);
#endif
                if (Directory.Exists(PluginsDirectory))
                {
                    IEnumerable<string> plugins =
                        Directory.GetFiles(PluginsDirectory).Where(f => f.Contains(".TicketFeed.dll"));
                    foreach (string file in plugins)
                    {
                        AppDomain.CurrentDomain.Load(File.ReadAllBytes(file));
#if DEBUG
                        System.Console.WriteLine("{0} loaded.", Path.GetFileName(file));
#endif
                    }
                }
            }
        }

        public static Output Output(string type) => CreatePluginItem<Output>(type) ?? new NoOutput();

        public static Source Source(string type) => CreatePluginItem<Source>(type) ?? new NoSource();

        private static T CreatePluginItem<T>(string type, params object[] parameters)
            where T : ILoadableItem
        {
            Type baseType = typeof(T);
            try
            {
                T[] outputs = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => baseType.IsAssignableFrom(p) && !p.IsAbstract)
                    .Select(p => Activator.CreateInstance(p, parameters))
                    .Cast<T>()
                    .ToArray();
                return outputs.FirstOrDefault(
                    o => o.Name.Trim().Equals(type, StringComparison.CurrentCultureIgnoreCase));
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
                return default(T);
            }
        }
    }
}