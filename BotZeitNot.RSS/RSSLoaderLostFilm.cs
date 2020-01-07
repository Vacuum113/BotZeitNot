using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Text;

namespace BotZeitNot.RSS
{
    public class RSSLoaderLostFilm
    {
        public string LoadingString { get; private set; }

        public RSSLoaderLostFilm(string loadingString)
        {
            LoadingString = loadingString;
        }

        public string LoadFromRSS()
        {
            SyndicationFeed feed = new SyndicationFeed("",)
        }
    }
}
