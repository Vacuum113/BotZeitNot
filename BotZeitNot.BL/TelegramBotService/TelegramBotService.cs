using BotZeitNot.BL.TelegramBotService.Answer;
using BotZeitNot.BL.TelegramBotService.Commands;
using BotZeitNot.BL.TelegramBotService.MassMailingNewEpisode;
using BotZeitNot.BL.TelegramBotService.TelegramBotConfig;
using BotZeitNot.Domain.Interface;
using BotZeitNot.Shared.Dto;
using Microsoft.Extensions.Logging;
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
        private ILogger<TelegramBotService> _logger;

        public TelegramBotService
            (
            ICommandList commandList,
            Bot bot,
            IUnitOfWorkFactory unitOfWorkFactory,
            ILogger<TelegramBotService> logger
            )
        {
            _commandList = commandList;
            _client = bot.Get();
            _unitOfWorkFactory = unitOfWorkFactory;
            _logger = logger;
        }


        public async Task Run(Update update)
        {
            _logger.LogInformation($"Time: {DateTime.UtcNow}. Entry to the webhook parsing method.");

            if (update == null)
            {
                _logger.LogWarning($"Time: {DateTime.UtcNow}. Empty \"Update\" obj.");
                return;
            }

            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        await IfMessage(update.Message);
                        break;
                    case UpdateType.CallbackQuery:
                        await IfCAllbackQuery(update);
                        break;
                    case UpdateType.EditedMessage:
                        await IfMessage(update.EditedMessage);
                        break;
                    default:
                        await Default(update);
                        break;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"Time: {DateTime.UtcNow}. Catch error in method - 'Run'. Error message: " + ex.Message);
            }
        }

        public void SendingNewSeries(IEnumerable<EpisodeDto> episodes)
        {
            _logger.LogInformation($"Time: {DateTime.UtcNow}. Entry in sending new series method.");

            if (episodes != null)
            {
                try
                {
                    new MassMailing(_unitOfWorkFactory, _client)
                        .SendingNewSeries(episodes);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Time: {DateTime.UtcNow}. Catch error in method - 'SendingNewSeries'. Error message: " + ex.Message);
                }
            }
            else
            {
                _logger.LogWarning($"Time: {DateTime.UtcNow}. Empty \"EpisodeDto\" obj.");
            }
        }


        private async Task IfMessage(Message message)
        {
            _logger.LogInformation($"Time: {DateTime.UtcNow}. If message type method.");
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
            else
            {
                if (message == null)
                {
                    _logger.LogWarning($"Time: {DateTime.UtcNow}. Empty \"Message\" obj.");
                }
                else if (message.From.IsBot)
                {
                    _logger.LogWarning($"Time: {DateTime.UtcNow}. Message from Bot.");
                }
                else if (message.Text == null)
                {
                    _logger.LogWarning($"Time: {DateTime.UtcNow}. Message text is null.");
                }
                else 
                {
                    _logger.LogWarning($"Time: {DateTime.UtcNow}. Wrong message text: {message.Text}.");
                }
                return;
            }
        }

        private async Task IfCAllbackQuery(Update update)
        {
            _logger.LogInformation($"Time: {DateTime.UtcNow}. If callbackquery type method.");

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
            else
            {
                if (update.CallbackQuery == null)
                {
                    _logger.LogWarning($"Time: {DateTime.UtcNow}. Empty \"CallbackQuery\" obj.");
                }
                else if (update.CallbackQuery.From.IsBot)
                {
                    _logger.LogWarning($"Time: {DateTime.UtcNow}. Message from Bot.");
                }
                else if (update.CallbackQuery.Message == null)
                {
                    _logger.LogWarning($"Time: {DateTime.UtcNow}. Message is null.");
                }
                return;
            }
        }


        private async Task Default(Update update)
        {
            _logger.LogInformation($"Time: {DateTime.UtcNow}. Default response method.");

            const string defaultString = "Извините, не понял вас.\n" +
                                         "Для просмотра списка команд - " +
                                         "отправте сообщение: \"/help\"\n " +
                                         "или напишите \"/\" для просмотра доступных команд.";

            await _client.SendTextMessageAsync(update.Message.Chat.Id, defaultString);
        }
    }
}
