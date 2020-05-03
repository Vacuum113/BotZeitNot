using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace BotZeitNot.RSS.RSSWorker
{
    public class RssLoaderLostFilm
    {
        private readonly string _loadingString;
        private readonly ILogger<RssLoaderLostFilm> _logger;

        public RssLoaderLostFilm(Settings settings)
        {
            _loadingString = settings.LostFilmRssLink;

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            _logger = loggerFactory.CreateLogger<RssLoaderLostFilm>();
        }

        public async Task<XmlReader> LoadFromRss()
        {
            string rssString;
            using (HttpClient client = new HttpClient())
            {
                var result = await client.GetAsync(_loadingString);
                var str = await result.Content.ReadAsStringAsync();
                StringBuilder stringBuilder = new StringBuilder(str);

                rssString = stringBuilder.Replace("0.91", "2.0").ToString();
            }

            return XmlReader.Create(new StringReader(rssString));
        }
    }
}
