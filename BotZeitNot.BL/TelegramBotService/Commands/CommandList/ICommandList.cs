using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotZeitNot.BL.TelegramBotService.Commands.CommandList
{
    public interface ICommandList
    {
        List<Command> Commands { get; }

        void Set(Command command);

        Command GetCommand(string commandMessage);
    }
}
