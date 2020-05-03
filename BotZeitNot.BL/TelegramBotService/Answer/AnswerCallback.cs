using BotZeitNot.BL.TelegramBotService.Helpers;
using BotZeitNot.DAL;
using BotZeitNot.DAL.Domain.Repositories;
using BotZeitNot.Domain.Interface;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotZeitNot.BL.TelegramBotService.Answer
{
    public class AnswerCallback
    {
        private readonly CallbackQuery _callbackQuery;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly TelegramBotClient _client;

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
            if (_callbackQuery.Data == "CancelAll")
            {
                using (var unitOfWork = _unitOfWorkFactory.Create())
                {
                    var subSeriesRepository = ((UnitOfWork)unitOfWork).SubSeries;

                    subSeriesRepository.CancelSubscriptionFromAll(_callbackQuery.From.Id);
                    unitOfWork.Save();
                }

                var cancelAllMessage = _callbackQuery.From.FirstName +
                    ", Вы отписались от рассылки всех сериалов";

                await MessageToTelegram.SendCallBackMessageTelegram(_callbackQuery, cancelAllMessage, _client);
            }
            else
            {
                using (var unitOfWork = _unitOfWorkFactory.Create())
                {
                    var nameRu = _callbackQuery.Data.Split("/")[1];
                    var subSeriesRepository = ((UnitOfWork)unitOfWork).SubSeries;

                    var errorMessage = $"Вы уже отписались от рассылки сериала - {nameRu}";
                    if (!subSeriesRepository.IsUserSubscribedToSeries(_callbackQuery.Message.Chat.Id, nameRu))
                    {
                        await MessageToTelegram.SendCallBackMessageTelegram(_callbackQuery, errorMessage, _client);
                        return;
                    }

                    subSeriesRepository.CancelSubscription(_callbackQuery.Message.Chat.Id, nameRu);

                    unitOfWork.Save();
                }
                var cancelSeriesMessage = _callbackQuery.From.FirstName +
                                          ", Вы отписались от рассылки сериала - " +
                                          _callbackQuery.Data.Split("/")[1];

                await MessageToTelegram.SendCallBackMessageTelegram(_callbackQuery, cancelSeriesMessage, _client);
            }
        }

        public async Task Search()
        {
            var chatId = _callbackQuery.Message.Chat.Id;
            var nameRu = _callbackQuery.Data.Split("/")[1];

            using (var unitOfWork = _unitOfWorkFactory.Create())
            {
                var subSeriesRepository = ((UnitOfWork)unitOfWork).SubSeries;

                var isExistSubscription = subSeriesRepository.IsUserSubscribedToSeries(chatId, nameRu);
                if (isExistSubscription)
                {
                    var errorMessage = $"Вы уже подписаны на {nameRu}";

                    await MessageToTelegram.SendCallBackMessageTelegram(_callbackQuery, errorMessage, _client);
                    return;
                }

                subSeriesRepository.AddSubscription(chatId, nameRu);

                unitOfWork.Save();
            }

            var newSeriesMessage = _callbackQuery.From.FirstName +
                                   ", Вы подписались на новые серии: " +
                                   _callbackQuery.Data.Split("/")[1];

            await MessageToTelegram.SendCallBackMessageTelegram(_callbackQuery, newSeriesMessage, _client);
        }
    }
}
