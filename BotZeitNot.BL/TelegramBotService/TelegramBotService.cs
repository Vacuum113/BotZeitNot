using BotZeitNot.BL.TelegramBotService.Commands;
using BotZeitNot.BL.TelegramBotService.Helpers;
using BotZeitNot.BL.TelegramBotService.TelegramBotConfig;
using BotZeitNot.DAL;
using BotZeitNot.DAL.Domain.Repositories;
using BotZeitNot.Domain.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotZeitNot.BL.TelegramBotService
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly ICommandList _commandList;

        private readonly TelegramBotClient _client;

        private IUnitOfWorkFactory _unitOfWorkFactory;

        public TelegramBotService
            (
            ICommandList commandList,
            Bot bot,
            IUnitOfWorkFactory unitOfWorkFactory
            )
        {
            _commandList = commandList;
            _client = bot.Get();
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task Run(Update update)
        {
            if (update == null)
            {
                throw new NullReferenceException();
            }

            switch (update.Type)
            {
                case UpdateType.Message:
                    await IfMessage(update.Message);
                    break;
                case UpdateType.CallbackQuery:
                    await IfCAllbackQuery(update);
                    break;
                default:
                    await Default(update);
                    break;
            }
        }


        public async Task IfMessage(Message message)
        {
            if (
                message != null &&
                !message.From.IsBot &&
                message.Text != null &&
                message.Text.StartsWith('/')
                )
            {
                Command command = _commandList.GetCommand(message.Text);

                if (command != null)
                    command.Execute(message, _client);
                else
                {
                    var helpString = "Для просмотра списка команд " +
                                     "- отправте сообщение: \"/help\"\n " +
                                     "или напишите \"/\" для " +
                                     "просмотра доступных команд.";

                    await _client.SendTextMessageAsync(message.Chat.Id, helpString);
                }
            }
        }

        public async Task IfCAllbackQuery(Update update)
        {
            if (
                update.CallbackQuery != null &&
                !update.CallbackQuery.From.IsBot &&
                update.CallbackQuery.Message != null
                )
                if (update.CallbackQuery.Data.Contains("Search"))
                {
                     await SubscriptionOnSeries(update.CallbackQuery);
                }
        }

        public async Task Default(Update update)
        {
            var defaultString = "Извините, не понял вас.\n" +
                                "Для просмотра списка команд - " +
                                "отправте сообщение: \"/help\"\n " +
                                "или напишите \"/\" для просмотра доступных команд.";

            await _client.SendTextMessageAsync(update.Message.Chat.Id, defaultString);
        }

        public async Task SubscriptionOnSeries(CallbackQuery callbackQuery)
        {
            long chatId = callbackQuery.Message.Chat.Id;
            string nameRu = callbackQuery.Data.Split("/")[1];

            using (IUnitOfWork unitOfWork = _unitOfWorkFactory.Create())
            {
                UserRepository userRepository = ((UnitOfWork)unitOfWork).Users;
                SeriesRepository seriesRepository = ((UnitOfWork)unitOfWork).Series;

                var user = userRepository.GetUserAndSeriesByTelegramId(callbackQuery.From.Id);

                if (user == default)
                {
                    string errorMessage = "Пользователь не был найден, " +
                                          "попробуйте начать с команды /start";

                    await MessageToTelegram.SendCallBackMessageTelegram(callbackQuery, errorMessage, _client);
                    return;
                }

                var userSeries = user.SubscriptionSeries.
                    Where(s => s.SeriesNameRu == nameRu).
                    FirstOrDefault();

                if (userSeries != default)
                {
                    string errorMessage = $"Вы уже подписаны на {nameRu}";

                    await MessageToTelegram.SendCallBackMessageTelegram(callbackQuery, errorMessage, _client);
                    return;
                }
                //bug
                var series = seriesRepository.GetByNameRuSeries(nameRu);

                userRepository.SubscriprionOnSeries(series, user);
                unitOfWork.Save();
            }

            var newSeriesMessage = callbackQuery.From.FirstName +
                                   ", Вы подписались на новые серии: " +
                                   callbackQuery.Data.Split("/")[1];

            await MessageToTelegram.SendCallBackMessageTelegram(callbackQuery, newSeriesMessage, _client);
        }

    }
}
