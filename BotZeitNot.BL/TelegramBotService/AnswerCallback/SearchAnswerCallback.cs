using BotZeitNot.BL.TelegramBotService.Helpers;
using BotZeitNot.DAL;
using BotZeitNot.DAL.Domain.Repositories;
using BotZeitNot.Domain.Interface;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotZeitNot.BL.TelegramBotService.AnswerCallback
{
    public class SearchAnswerCallback
    {
        public async Task SubscriptionOnSeries
            (
            CallbackQuery callbackQuery,
            IUnitOfWorkFactory unitOfWorkFactory,
            TelegramBotClient client
            )
        {
            long chatId = callbackQuery.Message.Chat.Id;
            string nameRu = callbackQuery.Data.Split("/")[1];

            using (IUnitOfWork unitOfWork = unitOfWorkFactory.Create())
            {
                UserRepository userRepository = ((UnitOfWork)unitOfWork).Users;
                SeriesRepository seriesRepository = ((UnitOfWork)unitOfWork).Series;

                var user = userRepository.GetUserAndSeriesByTelegramId(callbackQuery.From.Id);

                if (user == default)
                {
                    string errorMessage = "Пользователь не был найден, " +
                                          "попробуйте начать с команды /start";

                    await MessageToTelegram.SendCallBackMessageTelegram(callbackQuery, errorMessage, client);
                    return;
                }

                var userSeries = user.SubscriptionSeries.
                    Where(s => s.SeriesNameRu == nameRu).
                    FirstOrDefault();

                if (userSeries != default)
                {
                    string errorMessage = $"Вы уже подписаны на {nameRu}";

                    await MessageToTelegram.SendCallBackMessageTelegram(callbackQuery, errorMessage, client);
                    return;
                }

                var series = seriesRepository.GetByNameRuSeries(nameRu);

                userRepository.SubscriprionOnSeries(series, user);
                unitOfWork.Save();

                var newSeriesMessage = callbackQuery.From.FirstName +
                       ", Вы подписались на новые серии: " +
                       callbackQuery.Data.Split("/")[1];

                await MessageToTelegram.SendCallBackMessageTelegram(callbackQuery, newSeriesMessage, client);
            }
        }
    }
}
