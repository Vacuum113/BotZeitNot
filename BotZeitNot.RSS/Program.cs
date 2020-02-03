using BotZeitNot.RSS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Xml;

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
            var _logger = loggerFactory.CreateLogger<Program>();

            XmlReader xmlReader;
            List<Tuple<string, string>> episodesStrings;
            List<Episode> listEpisodes;
            List<Episode> listEpisodesPrev = new List<Episode>();

            using (FileStream fileStream = new FileStream("pervEpisodes.json", FileMode.OpenOrCreate))
            {
                if (fileStream.Length != 0)
                {
                    listEpisodesPrev = JsonSerializer.DeserializeAsync<List<Episode>>(fileStream).Result;
                }
            }

            _logger.LogInformation($"Time: {DateTime.UtcNow}. Start program");
            while (true)
            {
                try
                {
                    xmlReader = new RSSLoaderLostFilm(settings).LoadFromRSS().Result;

                    episodesStrings = new RSSParserLostFilm().ParseNamesAndLinks(xmlReader);

                    listEpisodes = new List<Episode>();

                    foreach (var item in episodesStrings)
                    {
                        Episode episode;

                        if (Episode.TryParse(new ParseAlgorithm(), item, out episode))
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
                    using (FileStream fileStream = new FileStream("pervEpisodes.json", FileMode.OpenOrCreate))
                    {
                        JsonSerializer.SerializeAsync(fileStream, listEpisodesPrev);
                    }

                    Thread.Sleep(TimeSpan.FromMinutes(7));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Time: { DateTime.UtcNow}. Catch error in method - 'Program'.Error message: " + ex.Message);
                    Thread.Sleep(TimeSpan.FromMinutes(2));
                }
            }

        }
    }
}
