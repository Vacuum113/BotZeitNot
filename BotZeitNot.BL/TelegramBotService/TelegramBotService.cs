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
                        await IfCAllbackQuery(update.CallbackQuery);
                        break;
                    case UpdateType.EditedMessage:
                        await IfMessage(update.EditedMessage);
                        break;
                    default:
                        await Default(update);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Time: {DateTime.UtcNow}. Catch error in method - 'Run'. Error message: " + ex.Message);
            }
        }

        public void SendingNewSeries(IEnumerable<EpisodeDto> episodes)
        {
            if (episodes != null)
            {
                try
                {
                    new MassMailing(_unitOfWorkFactory, _client).SendingNewSeries(episodes);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Time: {DateTime.UtcNow}. Catch error in method - 'SendingNewSeries'. Error message: " + ex.Message);
                }
            }
            else
            {
                _logger.LogWarning($"Time: {DateTime.UtcNow}. Empty \"EpisodeDto\" obj.");
            }
        }


        private async Task IfMessage(Message message)
        {
            bool messageNotNullAndIsNotBot = message != null && !message.From.IsBot;
            if (messageNotNullAndIsNotBot && message.Text != null)
            {
                Command command = _commandList.GetCommand(message.Text);
                bool isCommandNotNullAndIsMessageValid = command != null && message.Text.StartsWith('/');

                if (isCommandNotNullAndIsMessageValid)
                {
                    await command.Execute(message, _client);
                }
                else
                {
                    string helpString = "Для просмотра списка команд - отправте сообщение: \"/help\"\n " +
                                        "или напишите \"/\" для просмотра доступных команд.";

                    await _client.SendTextMessageAsync(message.Chat.Id, helpString);
                }
            }
            else
            {
                _logger.LogWarning($"Time: {DateTime.UtcNow}. Some problems with Message obj: " + message);
                return;
            }
        }

        private async Task IfCAllbackQuery(CallbackQuery callbackQuery)
        {
            bool callbackNotNullAndIsNotFromBot = callbackQuery != null && !callbackQuery.From.IsBot;

            if (callbackNotNullAndIsNotFromBot && callbackQuery.Message != null)
            {
                var answerCallback = new AnswerCallback(callbackQuery, _unitOfWorkFactory, _client);
                switch (callbackQuery.Data.Split("/")[0])
                {
                    case "Search":
                        await answerCallback.Search();
                        break;
                    case "Cancel":
                        await answerCallback.Cancel();
                        break;
                    case "CancelAll":
                        await answerCallback.Cancel();
                        break;
                }
            }
            else
            {
                _logger.LogWarning($"Time: {DateTime.UtcNow}. Some problems with CallbackQuery obj: " + callbackQuery);
                return;
            }
        }


        private async Task Default(Update update)
        {
            string defaultString = "Извините, не понял вас.\nДля просмотра списка команд - " +
                                   "отправте сообщение: \"/help\"\n или напишите \"/\" " +
                                   "для просмотра доступных команд.";

            switch (update.Type)
            {
                case UpdateType.EditedMessage:
                    await _client.SendTextMessageAsync(update.EditedMessage.From.Id, defaultString);
                    break;
                case UpdateType.Message:
                    await _client.SendTextMessageAsync(update.Message.From.Id, defaultString);
                    break;
                case UpdateType.InlineQuery:
                    await _client.SendTextMessageAsync(update.InlineQuery.From.Id, defaultString);
                    break;
                case UpdateType.EditedChannelPost:
                    await _client.SendTextMessageAsync(update.EditedChannelPost.From.Id, defaultString);
                    break;
                case UpdateType.ChannelPost:
                    await _client.SendTextMessageAsync(update.ChannelPost.From.Id, defaultString);
                    break;
                case UpdateType.CallbackQuery:
                    await _client.SendTextMessageAsync(update.CallbackQuery.From.Id, defaultString);
                    break;
                case UpdateType.ChosenInlineResult:
                    await _client.SendTextMessageAsync(update.ChosenInlineResult.From.Id, defaultString);
                    break;
                case UpdateType.PreCheckoutQuery:
                    await _client.SendTextMessageAsync(update.PreCheckoutQuery.From.Id, defaultString);
                    break;
                case UpdateType.ShippingQuery:
                    await _client.SendTextMessageAsync(update.ShippingQuery.From.Id, defaultString);
                    break;
            }
        }
    }
}
