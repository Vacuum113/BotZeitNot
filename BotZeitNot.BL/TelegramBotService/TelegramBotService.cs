using BotZeitNot.BL.TelegramBotService.Answer;
using BotZeitNot.BL.TelegramBotService.Commands;
using BotZeitNot.BL.TelegramBotService.MassMailingNewEpisode;
using BotZeitNot.BL.TelegramBotService.TelegramBotConfig;
using BotZeitNot.Domain.Interface;
using BotZeitNot.Shared.Dto;
using System;
using System.Collections.Generic;
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

        public void SendingNewSeries(IEnumerable<EpisodeDto> episodes)
        {
            new MassMailing(_unitOfWorkFactory, _client)
                .SendingNewSeries(episodes);
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
                    await command.Execute(message, _client);
                }
                else
                {
                    const string helpString = "Для просмотра списка команд " +
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
            {
                var answerCallback = new AnswerCallback(update.CallbackQuery, _unitOfWorkFactory, _client);
                switch (update.CallbackQuery.Data.Split("/")[0])
                {
                    case "Search":
                        await answerCallback
                            .Search();
                        break;

                    case "Cancel":
                        await answerCallback
                            .Cancel();
                        break;
                }
            }
        }


        private async Task Default(Update update)
        {
            const string defaultString = "Извините, не понял вас.\n" +
                                         "Для просмотра списка команд - " +
                                         "отправте сообщение: \"/help\"\n " +
                                         "или напишите \"/\" для просмотра доступных команд.";

            await _client.SendTextMessageAsync(update.Message.Chat.Id, defaultString);
        }
    }
}
