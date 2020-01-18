using BotZeitNot.BL.TelegramBotService.AnswerCallback;
using BotZeitNot.BL.TelegramBotService.Commands;
using BotZeitNot.BL.TelegramBotService.TelegramBotConfig;
using BotZeitNot.Domain.Interface;
using System;
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


        private async Task IfMessage(Message message)
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
                {
                    command.Execute(message, _client);
                }
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

        private async Task IfCAllbackQuery(Update update)
        {
            if (
                update.CallbackQuery != null &&
                !update.CallbackQuery.From.IsBot &&
                update.CallbackQuery.Message != null
                )
                switch(update.CallbackQuery.Data.Split("/")[0])
                {
                    case "Search":
                        await new SearchAnswerCallback()
                            .SubscriptionOnSeries(update.CallbackQuery, _unitOfWorkFactory, _client);
                        break;
                    case "Cancel":
                        await new CancelAnswerCallback()
                            .CancelSubscriptionOnSeries(update.CallbackQuery, _unitOfWorkFactory, _client);
                        break;
                }
        }


        private async Task Default(Update update)
        {
            var defaultString = "Извините, не понял вас.\n" +
                                "Для просмотра списка команд - " +
                                "отправте сообщение: \"/help\"\n " +
                                "или напишите \"/\" для просмотра доступных команд.";

            await _client.SendTextMessageAsync(update.Message.Chat.Id, defaultString);
        }
    }
}
