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
                using (IUnitOfWork unitOfWork = _unitOfWorkFactory.Create())
                {
                    SubSeriesRepository subSeriesRepository = ((UnitOfWork)unitOfWork).SubSeries;
                    UserRepository userRepository = ((UnitOfWork)unitOfWork).Users;

                    subSeriesRepository.CancelSubscriptionFromAll(_callbackQuery.From.Id);
                    unitOfWork.Save();
                }

                var cancelAllMessage = _callbackQuery.From.FirstName +
                    ", Вы отписались от рассылки всех сериалов";

                await MessageToTelegram.SendCallBackMessageTelegram(_callbackQuery, cancelAllMessage, _client);
            }
            else
            {
                using (IUnitOfWork unitOfWork = _unitOfWorkFactory.Create())
                {
                    string nameRu = _callbackQuery.Data.Split("/")[1];
                    SubSeriesRepository subSeriesRepository = ((UnitOfWork)unitOfWork).SubSeries;

                    if (!subSeriesRepository.IsUserSubscribedToSeries(_callbackQuery.Message.Chat.Id, nameRu))
                    {
                        string errorMessage = $"Вы уже отписались от рассылки сериала - {nameRu}";

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
            long chatId = _callbackQuery.Message.Chat.Id;
            string nameRu = _callbackQuery.Data.Split("/")[1];

            using (IUnitOfWork unitOfWork = _unitOfWorkFactory.Create())
            {
                SubSeriesRepository subSeriesRepository = ((UnitOfWork)unitOfWork).SubSeries;

                var IsExistSubscription = subSeriesRepository.IsUserSubscribedToSeries(chatId, nameRu);
                if (IsExistSubscription)
                {
                    string errorMessage = $"Вы уже подписаны на {nameRu}";

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
