using BotZeitNot.BL.TelegramBotService.Commands;
using BotZeitNot.BL.TelegramBotService.Commands.CommandList;
using BotZeitNot.BL.TelegramBotService.TelegramBotConfig;
using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotZeitNot.BL.TelegramBotService
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly ICommandList _commandList;

        private readonly TelegramBotClient _client;

        public TelegramBotService
            (
            ICommandList commandList,
            Bot bot
            )
        {
            _commandList = commandList;
            _client = bot.Get();
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
                    IfMessage(update);
                    break;
                case UpdateType.CallbackQuery:
                    IfCAllbackQuery(update);
                    break;
                default: 
                    Default(update);
                    break;
            }
        }

        public async void IfMessage(Update update)
        {
            if (
                update.Message != null &&
                !update.Message.From.IsBot &&
                update.Message.Text.StartsWith('/')
                )
            {
                Command command = _commandList.GetCommand(update.Message.Text);

                if (command != null)
                    command.Execute(update.Message, _client);
                else
                {
                    var helpString = "Для просмотра списка команд " +
                                     "- отправте сообщение: \"/help\"\n " +
                                     "или напишите \"/\" для " +
                                     "просмотра доступных команд.";

                    await _client.SendTextMessageAsync(update.Message.Chat.Id, helpString);
                }
            }
        }

        public async void IfCAllbackQuery(Update update)
        {
            if (
                update.CallbackQuery != null &&
                !update.CallbackQuery.From.IsBot &&
                update.CallbackQuery.Message != null
                )
            {
                if (update.CallbackQuery.Data.Contains("Search"))
                {
                    await _client.AnswerCallbackQueryAsync
                        (
                        update.CallbackQuery.Id
                        );

                    var newSeriesMessage = update.CallbackQuery.From.FirstName +
                        ", Вы подписались на новые серии: "
                        + update.CallbackQuery.Data.Split("/")[1];

                    await _client.SendTextMessageAsync
                        (
                        update.CallbackQuery.Message.Chat.Id,
                        newSeriesMessage
                        );

                    await _client.DeleteMessageAsync
                        (
                        update.CallbackQuery.Message.Chat.Id,
                        update.CallbackQuery.Message.MessageId
                        );
                }
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

    }
}
