using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;

namespace BotZeitNot.RSS
{
    public class RSSParserLostFilm
    {
        private ILogger<RSSParserLostFilm> _logger;

        public RSSParserLostFilm()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            _logger = loggerFactory.CreateLogger<RSSParserLostFilm>();
        }

        public List<Tuple<string, string>> ParseNamesAndLinks(XmlReader xmlReader)
        {
            _logger.LogInformation($"Time: {DateTime.UtcNow}. Start method parser rss");

            SyndicationFeed feed = SyndicationFeed.Load(xmlReader);

            List<Tuple<string, string>> tuples = new List<Tuple<string, string>>();

            foreach (var item in feed.Items)
            {
                tuples.Add(new Tuple<string, string>(item.Title.Text, item.Links[0].Uri.ToString()));
            }

            return tuples;
        }
    }
}
