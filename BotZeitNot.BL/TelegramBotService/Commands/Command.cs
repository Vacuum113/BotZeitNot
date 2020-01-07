using BotZeitNot.Domain.Interface;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotZeitNot.BL.TelegramBotService.Commands
{
    public abstract class Command
    {
        public abstract string Name { get;}

        public Command()
        {
            
        }

        public abstract void Execute(Message message, TelegramBotClient client);

        public bool Contains(string message)
        {
             return message.Equals(this.Name) ? true : false;
        }
    }
}
