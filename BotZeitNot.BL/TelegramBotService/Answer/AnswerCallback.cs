using BotZeitNot.BL.TelegramBotService.Helpers;
using BotZeitNot.DAL;
using BotZeitNot.DAL.Domain.Repositories;
using BotZeitNot.Domain.Interface;
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
        }


        public async Task Cancel()
        {
            long chatId = _callbackQuery.Message.Chat.Id;
            string nameRu = _callbackQuery.Data.Split("/")[1];

            using (IUnitOfWork unitOfWork = _unitOfWorkFactory.Create())
            {
                SubSeriesRepository subSeriesRepository = ((UnitOfWork)unitOfWork).SubSeries;
                UserRepository userRepository = ((UnitOfWork)unitOfWork).Users;

                var user = userRepository.GetUserAndSeriesByTelegramId(_callbackQuery.From.Id);

                if (CheckUserForEmpty(user).Result)
                {
                    return;
                }

                var userSeries = user.SubscriptionSeries.
                    Where(s => s.SeriesNameRu == nameRu).
                    FirstOrDefault();

                if (userSeries == default)
                {
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

                if (CheckUserForEmpty(user).Result)
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

        private async Task<bool> CheckUserForEmpty(User user)
        {
            if (user == default)
            {
                string errorMessage = "Пользователь не был найден, " +
                                      "попробуйте начать с команды /start";

                await MessageToTelegram.SendCallBackMessageTelegram(_callbackQuery, errorMessage, _client);
                return true;
            }
            return false;
        }
    }
}
