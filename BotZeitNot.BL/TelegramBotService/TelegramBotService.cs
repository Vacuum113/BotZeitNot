using BotZeitNot.BL.TelegramBotService.Commands;
using BotZeitNot.BL.TelegramBotService.Commands.CommandList;
using BotZeitNot.BL.TelegramBotService.TelegramBotConfig;
using BotZeitNot.DAL;
using BotZeitNot.DAL.Domain.Repositories;
using BotZeitNot.Domain.Interface;
using System;
using System.Linq;
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

        public void Run(Update update)
        {
            if (update == null)
            {
                throw new NullReferenceException();
            }

            switch (update.Type)
            {
                case UpdateType.Message:
                    IfMessage(update.Message);
                    break;
                case UpdateType.CallbackQuery:
                    IfCAllbackQuery(update);
                    break;
                default:
                    Default(update);
                    break;
            }
        }

        public async void IfMessage(Message message)
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

        public void IfCAllbackQuery(Update update)
        {
            if (
                update.CallbackQuery != null &&
                !update.CallbackQuery.From.IsBot &&
                update.CallbackQuery.Message != null
                )
                if (update.CallbackQuery.Data.Contains("Search"))
                {
                    SubscriptionOnSeries(update.CallbackQuery);
                }
        }

        public async void Default(Update update)
        {
            var defaultString = "Извините, не понял вас.\n" +
                                "Для просмотра списка команд - " +
                                "отправте сообщение: \"/help\"\n " +
                                "или напишите \"/\" для просмотра доступных команд.";

            await _client.SendTextMessageAsync(update.Message.Chat.Id, defaultString);
        }

        public async void SubscriptionOnSeries(CallbackQuery callbackQuery)
        {
            await _client.AnswerCallbackQueryAsync
                (
                callbackQuery.Id
                );

            using (IUnitOfWork unitOfWork = _unitOfWorkFactory.Create())
            {
                UserRepository userRepository = ((UnitOfWork)unitOfWork).Users;
                SeriesRepository seriesRepository = ((UnitOfWork)unitOfWork).Series;

                var user = userRepository.GetUserAndSeriesByTelegramId(callbackQuery.From.Id);

                if (user == default)
                {
                    string errorMessage = "Пользователь не был найден, " +
                                          "попробуйте начать с команды /start";

                    await _client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, errorMessage);
                    return;
                }

                string nameRu = callbackQuery.Data.Split("/")[1];

                var userSeries = user.
                    SeriesUser.
                    Where(su => su.Series.NameRu == nameRu).
                    Select(su => su.Series).
                    FirstOrDefault();

                if (userSeries == default)
                {
                    string errorMessage = "Вы уже подписаны на " + nameRu;

                    await _client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, errorMessage);
                    return;
                }

                var series = seriesRepository.GetByNameRuSeries(nameRu);

                if (!userRepository.SubscriprionOnSeries(series, user))
                {
                    string errorMessage = "Произошла ошибка на стороне сервера повторите попытку";

                    await _client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, errorMessage);
                    return;
                }

                unitOfWork.Save();
            }

            var newSeriesMessage = callbackQuery.From.FirstName +
                ", Вы подписались на новые серии: "
                + callbackQuery.Data.Split("/")[1];

            await _client.SendTextMessageAsync
                (
                callbackQuery.Message.Chat.Id,
                newSeriesMessage
                );

            await _client.DeleteMessageAsync
                (
                callbackQuery.Message.Chat.Id,
                callbackQuery.Message.MessageId
                );
        }

    }
}
