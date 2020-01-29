using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Message = Telegram.Bot.Types.Message;

namespace BotZeitNot.BL.TelegramBotService.Commands.List
{
    public class HelpCommand : Command
    {
        public override string Name => "/help";

        private readonly List<Command> _commandList;
        private ILogger<HelpCommand> _logger;

        public HelpCommand(List<Command> commandList)
        {
            _commandList = commandList;

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            _logger = loggerFactory.CreateLogger<HelpCommand>();
        }

        public async override Task Execute(Message message, TelegramBotClient client)
        {
            _logger.LogInformation($"Time: {DateTime.UtcNow}. Execute help command.");

            var helpString = new StringBuilder("Все команды бота:\n");

            foreach (var command in _commandList)
            {
                if (command.Name != "/help")
                {
                    helpString.Append(command.Name + '\n');
                }
            }

            await client.SendTextMessageAsync(message.Chat.Id, helpString.ToString());
        }
    }
}
