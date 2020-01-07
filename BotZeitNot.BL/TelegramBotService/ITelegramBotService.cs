using BotZeitNot.Domain.Interface;
using Telegram.Bot.Types;

namespace BotZeitNot.BL.TelegramBotService
{
    public interface ITelegramBotService : IService
    {
        void ExecuteCommand(Update update);
    }
}
