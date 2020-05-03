using BotZeitNot.RSS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Xml;
using BotZeitNot.RSS.Model.Algorithm;
using BotZeitNot.RSS.RSSWorker;

namespace BotZeitNot.RSS
{
    class Program
    {
        private static IConfiguration Configuration;

        static void Main(string[] args)
        {
            Configure.ConfigurationFromFile();
            Configuration = Configure.Configuration;
            var settings = Configuration.
                                    GetSection("Settings")
                                    .Get<Settings>();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            var logger = loggerFactory.CreateLogger<Program>();

            var listEpisodesPrev = new List<Episode>();

            using (var fileStream = new FileStream("pervEpisodes.json", FileMode.OpenOrCreate))
            {
                if (fileStream.Length != 0)
                {
                    listEpisodesPrev = JsonSerializer.DeserializeAsync<List<Episode>>(fileStream).Result;
                }
            }

            while (true)
            {
                try
                {
                    var xmlReader = new RssLoaderLostFilm(settings).LoadFromRss().Result;

                    var episodesStrings = new RssParserLostFilm().ParseNamesAndLinks(xmlReader);

                    var listEpisodes = new List<Episode>();

                    foreach (var item in episodesStrings)
                    {
                        if (Episode.TryParse(new ParseAlgorithm(), item, out var episode))
                        {
                            listEpisodes.Add(episode);
                        }
                    }

                    var exporter = new Exporter(settings);

                    while (!exporter.Export(listEpisodes, listEpisodesPrev))
                    {
                        Thread.Sleep(TimeSpan.FromMinutes(2));
                    }
                    listEpisodesPrev = listEpisodes;

                    File.WriteAllText("pervEpisodes.json", string.Empty);
                    using (var fileStream = new FileStream("pervEpisodes.json", FileMode.OpenOrCreate))
                    {
                        JsonSerializer.SerializeAsync(fileStream, listEpisodesPrev);
                    }

                    Thread.Sleep(TimeSpan.FromMinutes(7));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Time: { DateTime.UtcNow}. Catch error in method - 'Program'.Error message: " + ex.Message);
                    Thread.Sleep(TimeSpan.FromMinutes(2));
                }
            }
        }
    }
}
