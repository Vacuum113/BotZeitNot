using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BotZeitNot.RSS
{
    public class RSSLoaderLostFilm
    {
        private string _loadingString;
        private ILogger<RSSLoaderLostFilm> _logger;

        public RSSLoaderLostFilm(Settings settings)
        {
            _loadingString = settings.LostFilmRSSLink;

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            _logger = loggerFactory.CreateLogger<RSSLoaderLostFilm>();
        }

        public async Task<XmlReader> LoadFromRSS()
        {
            _logger.LogInformation($"Time: {DateTime.UtcNow}. Start method loader rss");


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
