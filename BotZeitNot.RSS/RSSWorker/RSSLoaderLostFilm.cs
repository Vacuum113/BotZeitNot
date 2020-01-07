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
        public string LoadingString { get; private set; }

        public RSSLoaderLostFilm(string loadingString)
        {
            LoadingString = loadingString;
        }

        public async Task<XmlReader> LoadFromRSS(Settings settings)
        {
            IWebProxy proxy = new HttpToSocks5Proxy
                (
                settings.SocksIP,
                settings.SocksPort,
                settings.SocksUser,
                settings.SocksPassword
                );

            var clientHandler = new HttpClientHandler()
            {
                Proxy = proxy
            };

            string rssString;
            using (HttpClient client = new HttpClient(clientHandler, disposeHandler: true))
            {
                var result = await client.GetAsync(LoadingString);
                var str = await result.Content.ReadAsStringAsync();
                StringBuilder stringBuilder = new StringBuilder(str);

                rssString = stringBuilder.Replace("0.91", "2.0").ToString();
            }

            return XmlReader.Create(new StringReader(rssString));
        }
    }
}
