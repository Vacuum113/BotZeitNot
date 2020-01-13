using System.Collections.Generic;

namespace BotZeitNot.BL.TelegramBotService.Commands
{
    public interface ICommandList
    {
        List<Command> Commands { get; }

        void Set(Command command);

        Command GetCommand(string commandMessage);
    }
}
