using BotZeitNot.Domain.Interface;
using BotZeitNot.Shared.Dto;
using Telegram.Bot.Types;

namespace BotZeitNot.BL.TelegramBotService
{
    public interface ITelegramBotService : IService
    {
        void ExecuteCommand(Update update);
    }
}
