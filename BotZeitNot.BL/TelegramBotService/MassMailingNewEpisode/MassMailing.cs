using BotZeitNot.DAL;
using BotZeitNot.DAL.Domain.Entity;
using BotZeitNot.DAL.Domain.Repositories;
using BotZeitNot.Domain.Interface;
using BotZeitNot.Shared.Dto;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotZeitNot.BL.TelegramBotService.MassMailingNewEpisode
{
    public class MassMailing
    {
        private readonly SubSeriesRepository _subSeriesRepository;
        private readonly SeriesRepository _seriesRepository;
        private readonly TelegramBotClient _client;

        public MassMailing(IUnitOfWorkFactory unitOfWorkFactory, TelegramBotClient client)
        {
            _subSeriesRepository = ((UnitOfWork)unitOfWorkFactory.Create()).SubSeries;
            _seriesRepository = ((UnitOfWork)unitOfWorkFactory.Create()).Series;
            _client = client;
        }

        public void SendingNewSeries(IEnumerable<EpisodeDto> episodes)
        {
            foreach (var episode in episodes)
            {
                if (_seriesRepository.GetByNameRuSeries(episode.TitleSeries) == default)
                {
                    var seriesLink = episode.Link.Split("season")[0] + "seasons";

                    _seriesRepository.Add(new Series
                    {
                        IsCompleted = false,
                        NameEn = episode.TitleSeriesEn,
                        Link = seriesLink,
                        NameRu = episode.TitleSeries,
                        SeasonsCount = episode.NumberSeason
                    });
                    continue;
                }

                var chatIdQueue = MakeQueueChatId(episode.TitleSeries);

                var countRequests = 0;

                while (chatIdQueue.Count != 0)
                {
                    var chatId = chatIdQueue.Dequeue();

                    countRequests = SendNewEpisode(countRequests, episode, chatId).Result;

                    if (countRequests < 20) 
                        continue;
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    countRequests = 0;
                }
            }
        }

        private Queue<long> MakeQueueChatId(string titleRu)
        {
            var chatIdQueue = new Queue<long>();
            var chatIdArray = _subSeriesRepository.GetChatIdBySeriesNameRu(titleRu);
            foreach (var chatId in chatIdArray)
            {
                chatIdQueue.Enqueue(chatId);
            }
            return chatIdQueue;
        }

        private async Task<int> SendNewEpisode(int count, EpisodeDto episodeDto, long chatId)
        {
            if (count >= 20)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                count = 0;
            }

            var notifyMessage = "Вышла новая серия!\nСериал: " + episodeDto.TitleSeries +
                                "\nСерия: " + episodeDto.TitleRu +
                                "\nСсылка на серию: " + episodeDto.Link;

            var message = await _client.SendTextMessageAsync(chatId, notifyMessage);
            count++;

            if (message.Chat.Description == null) 
                return count;
            Thread.Sleep(TimeSpan.FromSeconds(2));
            await _client.SendTextMessageAsync(chatId, notifyMessage);

            return count;
        }
    }
}
