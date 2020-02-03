using BotZeitNot.DAL;
using BotZeitNot.DAL.Domain.Entity;
using BotZeitNot.DAL.Domain.Repositories;
using BotZeitNot.Domain.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotZeitNot.BL.TelegramBotService.Commands.List
{
    class SearchCommand : Command
    {
        public override string Name => "/search";

        private SeriesRepository _seriesRepository;
        private Message _message;
        private TelegramBotClient _client;

        public SearchCommand(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _seriesRepository = ((UnitOfWork)unitOfWorkFactory.Create()).Series;
        }

        public async override Task Execute(Message message, TelegramBotClient client)
        {
            _client = client;
            _message = message;

            if (IsTextStringLessThan8Char(_message.Text).Result)
            {
                return;
            }

            string text = message.Text.
                Remove(0, 8).
                TrimStart();

            if (!IsNameSeriesValid(text).Result)
            {
                return;
            }

            List<Series> series = _seriesRepository.GetByNameAllMatchSeries(text).ToList();
            if (!IsSeriesListValid(series).Result)
            {
                return;
            }

            int countSeries = series.
                FindAll(s => s.IsCompleted == true).
                Count;

            if (countSeries != 0)
            {
                var completedSeries = new StringBuilder();
                completedSeries.Append("Серилал(ы) завершившийся(еся):\n");
                foreach (var item in series)
                {
                    if (item.IsCompleted)
                    {
                        completedSeries.Append(item.NameRu + ", ");
                    }
                }

                int startIndex = completedSeries.Length - 2;
                string completedSeriesString = completedSeries
                    .Remove(startIndex, 2)
                    .Append(".")
                    .ToString();

                await client.SendTextMessageAsync(message.Chat.Id, completedSeriesString);

                if (series.FindAll(s => s.IsCompleted == true).Count == series.Count)
                    return;
            }

            await SendSearchButtons(series);
        }


        private async Task<bool> IsTextStringLessThan8Char(string message)
        {
            if (message.Length < 8)
            {
                string errorMessage = "Что бы найти сериал введите, " +
                                      "без кавычек: /search <Название Сериала>";

                await _client.SendTextMessageAsync(_message.Chat.Id, errorMessage);
                return true;
            }
            return false;
        }

        private async Task<bool> IsNameSeriesValid(string text)
        {
            if (text == "")
            {
                string errorMessage = "Что бы найти сериал, введите, " +
                                      "без кавычек: /search <Название Сериала>";

                await _client.SendTextMessageAsync(_message.Chat.Id, errorMessage);
                return false;
            }
            return true;
        }

        private async Task<bool> IsSeriesListValid(List<Series> series)
        {
            if (series == null || series.Count == 0)
            {
                string errorMessage = "Не найден такой сериал.\n" +
                                      "Либо неверно введено название " +
                                      "на русском/английском, " +
                                      "либо такого сериала нет на LostFilm.";

                await _client.SendTextMessageAsync(_message.Chat.Id, errorMessage);
                return false;
            }
            return true;
        }

        private async Task SendSearchButtons(List<Series> series)
        {
            var buttons = new List<InlineKeyboardButton>();
            foreach (var item in series)
            {
                if (item.IsCompleted != true)
                {
                    buttons.Add(new InlineKeyboardButton()
                    {
                        Text = item.NameRu,
                        CallbackData = "Search/" + item.NameRu
                    });
                }
            }

            await _client.SendTextMessageAsync
                (
                chatId: _message.Chat.Id,
                text: "Найденый(ыe) сериал(ы): ",
                replyMarkup: new InlineKeyboardMarkup(buttons)
                );
        }
    }
}
