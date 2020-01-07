using MihaZupan;
using Telegram.Bot;

namespace BotZeitNot.BL.TelegramBotService.TelegramBotConfig
{
    public class Bot
    {
        private readonly TelegramBotClient _client;

        public Bot(BotConfigProps botConfig)
        {
            var proxy = new HttpToSocks5Proxy
                (
                botConfig.SocksIP,
                botConfig.SocksPort,
                botConfig.SocksUser,
                botConfig.SocksPassword
                );

            _client = new TelegramBotClient(botConfig.Token, proxy);
        }

        public async void Run(string responseUrl)
        {
            await _client.SetWebhookAsync(responseUrl);
        }

        public TelegramBotClient Get() => _client;
    }
}
