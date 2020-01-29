﻿using BotZeitNot.DAL;
using BotZeitNot.DAL.Domain.Repositories;
using BotZeitNot.Domain.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = BotZeitNot.DAL.Domain.Entity.User;

namespace BotZeitNot.BL.TelegramBotService.Commands.List
{
    public class CancelCommand : Command
    {
        public override string Name => "/cancel";


        private UserRepository _userRepository;
        private TelegramBotClient _client;
        private Message _message;
        private ILogger<CancelCommand> _logger;

        public CancelCommand(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _userRepository = ((UnitOfWork)unitOfWorkFactory.Create()).Users;

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            _logger = loggerFactory.CreateLogger<CancelCommand>();
        }

        public async override Task Execute(Message message, TelegramBotClient client)
        {
            _logger.LogInformation($"Time: {DateTime.UtcNow}. Execute cancel command.");

            _client = client;
            _message = message;

            var user = _userRepository.GetUserAndSeriesByTelegramId(message.From.Id);
            if (!IsUserValid(user).Result)
            {
                _logger.LogWarning($"Time: {DateTime.UtcNow}. User not found.");
                return;
            }

            List<string> series = user.SubscriptionSeries
                .Select(s => s.SeriesNameRu)
                .ToList();

            if (!IsSubSeriesValid(series).Result)
            {
                _logger.LogInformation($"Time: {DateTime.UtcNow}. User subscription series not found.");
                return;
            }

            await SendCancelButtons(series);
        }


        private async Task<bool> IsUserValid(User user)
        {
            if (user == default)
            {
                string errorMessage = "Пользователь не найден, " +
                                      "попробуйте начать с команды /start";

                await _client.SendTextMessageAsync(_message.Chat.Id, errorMessage);
                return false;
            }
            return true;
        }

        private async Task<bool> IsSubSeriesValid(List<string> series)
        {
            if (series == null || series.Count == 0)
            {
                string errorMessage = "У вас еще нет подписок.\n" +
                                      "Найти и добавить сериалы " +
                                      "можно с помощью команды /search";

                await _client.SendTextMessageAsync(_message.Chat.Id, errorMessage);
                return false;
            }
            return true;
        }

        private async Task SendCancelButtons(List<string> series)
        {
            string cancelMessage = "Выберите, от рассылки какого " +
                                   "сериала вы хотите отписаться";

            var buttons = series.Select(s => new InlineKeyboardButton()
            {
                Text = s,
                CallbackData = "Cancel/" + s
            });

            await _client.SendTextMessageAsync
                (
                _message.Chat.Id,
                cancelMessage,
                replyMarkup: new InlineKeyboardMarkup(buttons)
                );
        }
    }
}
