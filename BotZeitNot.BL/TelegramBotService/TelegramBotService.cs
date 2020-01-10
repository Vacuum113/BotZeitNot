using BotZeitNot.BL.TelegramBotService.Commands;
using BotZeitNot.BL.TelegramBotService.Commands.CommandList;
using BotZeitNot.BL.TelegramBotService.TelegramBotConfig;
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

        public async void ExecuteCommand(Update update)
        {
            if (
                update.Message.Text.StartsWith('/') &&
                update.Message != null &&
                !update.Message.From.IsBot &&
                update.Type == UpdateType.Message
                )
            {
                Command command = _commandList.GetCommand(update.Message.Text);

                if (command != null)
                    command.Execute(update.Message, _client);
                else
                    await _client.SendTextMessageAsync(update.Message.Chat.Id,
                        $"Для просмотра списка команд - отправте сообщение: \"/help\"\n или напишите \"/\" для просмотра доступных команд.");
            }
            else
            {
                await _client.SendTextMessageAsync(update.Message.Chat.Id, "Извините, не понял вас.\n" +
                    $"Для просмотра списка команд - отправте сообщение: \"/help\"\n или напишите \"/\" для просмотра доступных команд.");
            }
        }
    }
}
