using BotZeitNot.BL.TelegramBotService.Helpers;
using BotZeitNot.DAL;
using BotZeitNot.DAL.Domain.Repositories;
using BotZeitNot.Domain.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = BotZeitNot.DAL.Domain.Entity.User;

namespace BotZeitNot.BL.TelegramBotService.Answer
{
    public class AnswerCallback
    {
        private CallbackQuery _callbackQuery;
        private IUnitOfWorkFactory _unitOfWorkFactory;
        private TelegramBotClient _client;
        private ILogger<AnswerCallback> _logger;

        public AnswerCallback
            (
            CallbackQuery callbackQuery,
            IUnitOfWorkFactory unitOfWorkFactory,
            TelegramBotClient client
            )
        {
            _callbackQuery = callbackQuery;
            _unitOfWorkFactory = unitOfWorkFactory;
            _client = client;

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            _logger = loggerFactory.CreateLogger<AnswerCallback>();
        }


        public async Task Cancel()
        {
            _logger.LogInformation($"Time: {DateTime.UtcNow}. AnswerCallback Cancel method.");

            long chatId = _callbackQuery.Message.Chat.Id;
            string nameRu = _callbackQuery.Data.Split("/")[1];
            

            using (IUnitOfWork unitOfWork = _unitOfWorkFactory.Create())
            {
                SubSeriesRepository subSeriesRepository = ((UnitOfWork)unitOfWork).SubSeries;
                UserRepository userRepository = ((UnitOfWork)unitOfWork).Users;

                var user = userRepository.GetUserAndSeriesByTelegramId(_callbackQuery.From.Id);

                if (!IsUserValid(user).Result)
                {
                    _logger.LogWarning($"Time: {DateTime.UtcNow}. User is null.");
                    return;
                }

                var userSeries = user.SubscriptionSeries.
                    Where(s => s.SeriesNameRu == nameRu).
                    FirstOrDefault();

                if (userSeries == default)
                {
                    _logger.LogWarning
                        (
                        $"Time: {DateTime.UtcNow}. " +
                        $"The user has already unsubscribed from the series."
                        );

                    string errorMessage = $"Вы уже отписались от рассылки сериала - {nameRu}";

                    await MessageToTelegram.SendCallBackMessageTelegram(_callbackQuery, errorMessage, _client);
                    return;
                }

                subSeriesRepository.CancelSubscription(user.ChatId, nameRu);
                unitOfWork.Save();
            }

            var newSeriesMessage = _callbackQuery.From.FirstName +
                   ", Вы отписались от рассылки сериала - " +
                   _callbackQuery.Data.Split("/")[1];

            await MessageToTelegram.SendCallBackMessageTelegram(_callbackQuery, newSeriesMessage, _client);

        }

        public async Task Search()
        {
            long chatId = _callbackQuery.Message.Chat.Id;
            string nameRu = _callbackQuery.Data.Split("/")[1];

            using (IUnitOfWork unitOfWork = _unitOfWorkFactory.Create())
            {
                UserRepository userRepository = ((UnitOfWork)unitOfWork).Users;
                SeriesRepository seriesRepository = ((UnitOfWork)unitOfWork).Series;

                var user = userRepository.GetUserAndSeriesByTelegramId(_callbackQuery.From.Id);

                if (!IsUserValid(user).Result)
                {
                    return;
                }

                var userSeries = user.SubscriptionSeries.
                    Where(s => s.SeriesNameRu == nameRu).
                    FirstOrDefault();

                if (userSeries != default)
                {
                    string errorMessage = $"Вы уже подписаны на {nameRu}";

                    await MessageToTelegram.SendCallBackMessageTelegram(_callbackQuery, errorMessage, _client);
                    return;
                }

                var series = seriesRepository.GetByNameRuSeries(nameRu);

                userRepository.SubscriprionOnSeries(series, user);
                unitOfWork.Save();
            }

            var newSeriesMessage = _callbackQuery.From.FirstName +
                   ", Вы подписались на новые серии: " +
                   _callbackQuery.Data.Split("/")[1];

            await MessageToTelegram.SendCallBackMessageTelegram(_callbackQuery, newSeriesMessage, _client);
        }

        private async Task<bool> IsUserValid(User user)
        {
            if (user == default)
            {
                string errorMessage = "Пользователь не был найден, " +
                                      "попробуйте начать с команды /start";

                await MessageToTelegram.SendCallBackMessageTelegram(_callbackQuery, errorMessage, _client);
                return false;
            }
            return true;
        }
    }
}
