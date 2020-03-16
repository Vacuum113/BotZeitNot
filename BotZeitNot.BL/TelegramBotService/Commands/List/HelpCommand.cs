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

        public HelpCommand(List<Command> commandList)
        {
            _commandList = commandList;
        }

        public async override Task Execute(Message message, TelegramBotClient client)
        {
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
