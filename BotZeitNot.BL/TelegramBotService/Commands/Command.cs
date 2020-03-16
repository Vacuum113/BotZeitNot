using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotZeitNot.BL.TelegramBotService.Commands
{
    public abstract class Command
    {
        public abstract string Name { get; }

        public Command() { }

        public abstract Task Execute(Message message, TelegramBotClient client);

        public bool Contains(string message) => message.Equals(this.Name.ToLower());
    }
}
