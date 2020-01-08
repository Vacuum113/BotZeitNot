using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;

namespace BotZeitNot.RSS
{
    public class RSSParserLostFilm
    {
        public List<Tuple<string, string>> ParseNamesAndLinks(XmlReader xmlReader)
        {
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
