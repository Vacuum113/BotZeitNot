using System.Text;
using Telegram.Bot;
using Message = Telegram.Bot.Types.Message;

namespace BotZeitNot.BL.TelegramBotService.Commands.CommandList.List
{
    public class HelpCommand : Command
    {
        public override string Name => "/help";

        private readonly ICommandList _commandList;

        public HelpCommand(ICommandList commandList)
        {
            _commandList = commandList;
        }

        public async override void Execute(Message message, TelegramBotClient client)
        {
            var helpString = new StringBuilder("Все команды бота:\n");

            foreach (var command in _commandList.Commands)
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
