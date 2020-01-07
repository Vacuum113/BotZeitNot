using BotZeitNot.RSS.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace BotZeitNot.RSS
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var settings = Configuration.GetSection("Settings").Get<Settings>();

            var xmlReader = new RSSLoaderLostFilm(settings.LostFilmRSSLink).LoadFromRSS(settings).Result;

            var episodes = new RSSParserLostFilm().ParseNamesAndLinks(xmlReader);

            var listEpisodes = new List<Episode>();

            foreach (var item in episodes)
            {
                Episode episode;

                if (Episode.TryParse(new ParseAlgorithm(), item, out episode))
                {
                    listEpisodes.Add(episode);
                }
            }

            Console.ReadKey();

        }
    }
}
