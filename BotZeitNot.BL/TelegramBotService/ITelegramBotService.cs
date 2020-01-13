using BotZeitNot.Domain.Interface;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotZeitNot.BL.TelegramBotService
{
    public interface ITelegramBotService : IService
    {
        Task Run(Update update);
    }
}
