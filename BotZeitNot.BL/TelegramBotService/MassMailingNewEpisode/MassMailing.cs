using BotZeitNot.DAL;
using BotZeitNot.DAL.Domain.Entity;
using BotZeitNot.DAL.Domain.Repositories;
using BotZeitNot.Domain.Interface;
using BotZeitNot.Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace BotZeitNot.BL.TelegramBotService.MassMailingNewEpisode
{
    public class MassMailing
    {
        private SubSeriesRepository _subSeriesRepository;
        private SeriesRepository _seriesRepository;
        private IUnitOfWorkFactory _unitOfWorkFactory;
        private TelegramBotClient _client;

        public MassMailing(IUnitOfWorkFactory unitOfWorkFactory, TelegramBotClient client)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
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
                    string seriesLink = episode.Link.Split("season")[0] + "seasons";

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

                Queue<long> chatIdQueue = MakeQueueChatId(episode.TitleSeries);

                int countRequests = 0;

                while (chatIdQueue.Count != 0)
                {
                    long chatId = chatIdQueue.Dequeue();

                    countRequests = SendNewEpisode(countRequests, episode, chatId).Result;

                    if (countRequests >= 20)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        countRequests = 0;
                    }
                }
            }
        }

        private Queue<long> MakeQueueChatId(string titleRu)
        {
            Queue<long> chatIdQueue = new Queue<long>();
            long[] chatIdArray = _subSeriesRepository.GetChatIdBySeriesNameRu(titleRu);
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

            string notifyMessage = "Вышла новая серия!\nСериал: " + episodeDto.TitleSeries +
                                   "\nСерия: " + episodeDto.TitleRu +
                                   "\nСсылка на серию: " + episodeDto.Link;

            Message message = await _client.SendTextMessageAsync(chatId, notifyMessage);
            count++;

            if (message.Chat.Description != null)
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                await _client.SendTextMessageAsync(chatId, notifyMessage);
            }

            return count;
        }
    }
}
