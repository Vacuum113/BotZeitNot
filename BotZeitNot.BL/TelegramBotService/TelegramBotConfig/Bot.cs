﻿using MihaZupan;
using System;
using System.IO;
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
                botConfig.SocksIp,
                botConfig.SocksPort,
                botConfig.SocksUser,
                botConfig.SocksPassword
                );

            _client = new TelegramBotClient(botConfig.Token, proxy);
        }

        public Bot(string token) => _client = new TelegramBotClient(token);

        public async void Run(string responseUrl)
        {
            if (!File.Exists("public.pem"))
            {
                var fs = File.OpenRead("public.pem");
                await _client.SetWebhookAsync(responseUrl, fs);
            }
            else
            {
                await _client.SetWebhookAsync(responseUrl);
            }
        }

        public TelegramBotClient Get() => _client;
    }
}
