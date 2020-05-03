using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace BotZeitNot.RSS.RSSWorker
{
    public class RssParserLostFilm
    {
        private readonly ILogger<RssParserLostFilm> _logger;

        public RssParserLostFilm()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            _logger = loggerFactory.CreateLogger<RssParserLostFilm>();
        }

        public List<Tuple<string, string>> ParseNamesAndLinks(XmlReader xmlReader)
        {
            var feed = SyndicationFeed.Load(xmlReader);

            return feed.Items.Select(item => new Tuple<string, string>(item.Title.Text, item.Links[0].Uri.ToString())).ToList();
        }
    }
}
