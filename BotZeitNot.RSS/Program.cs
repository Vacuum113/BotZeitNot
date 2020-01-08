using BotZeitNot.RSS.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;

namespace BotZeitNot.RSS
{
    class Program
    {
        private static IConfiguration Configuration;

        static void Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var settings = Configuration.GetSection("Settings").Get<Settings>();

            XmlReader xmlReader;
            List<Tuple<string, string>> episodesStrings;
            List<Episode> listEpisodes;
            List<Episode> listEpisodesPrev = new List<Episode>();

            while (true)
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

                while(!exporter.Export(listEpisodes, listEpisodesPrev))
                {
                    Thread.Sleep(TimeSpan.FromMinutes(2));
                }

                listEpisodesPrev = listEpisodes;

                Thread.Sleep(TimeSpan.FromMinutes(7));

            }
        }
    }
}
