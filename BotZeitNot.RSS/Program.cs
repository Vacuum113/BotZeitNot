using BotZeitNot.RSS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

            _logger.LogInformation($"Time: {DateTime.UtcNow}. Start program");

            while (true)
            {
                try
                {
                    _logger.LogInformation($"Time: {DateTime.UtcNow}. New cycle pass");


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
                        _logger.LogInformation($"Time: {DateTime.UtcNow}. ListEpisodes count - {listEpisodes.Count}. ListEpisodesPrev count - {listEpisodesPrev}");
                        _logger.LogInformation($"Time: {DateTime.UtcNow}. Sleep. The exporter object returned false");

                        Thread.Sleep(TimeSpan.FromMinutes(2));
                    }

                    listEpisodesPrev = listEpisodes;

                    _logger.LogInformation($"Time: {DateTime.UtcNow}. Sleep. After export new episodes.");
                    Thread.Sleep(TimeSpan.FromMinutes(7));
                }
                catch (Exception ex)
                {
                    Thread.Sleep(TimeSpan.FromMinutes(2));
                    _logger.LogError(ex, $"Time: { DateTime.UtcNow}. Catch error in method - 'Program'.Error message: " + ex.Message);
                }
            }

        }
    }
}
