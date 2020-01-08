using MihaZupan;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BotZeitNot.RSS
{
    public class RSSLoaderLostFilm
    {
        private string _loadingString;

        private HttpClientHandler _clientHandler;

        public RSSLoaderLostFilm(Settings settings)
        {
            _loadingString = settings.LostFilmRSSLink;

            IWebProxy proxy = new HttpToSocks5Proxy
                (
                settings.SocksIP,
                settings.SocksPort,
                settings.SocksUser,
                settings.SocksPassword
                );

            _clientHandler = new HttpClientHandler()
            {
                Proxy = proxy
            };
        }

        public async Task<XmlReader> LoadFromRSS()
        {

            string rssString;
            using (HttpClient client = new HttpClient(_clientHandler, disposeHandler: true))
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
