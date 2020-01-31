using BotZeitNot.Domain.Interface;
using BotZeitNot.Shared.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotZeitNot.BL.TelegramBotService
{
    public interface ITelegramBotService
    {
        Task Run(Update update);
        void SendingNewSeries(IEnumerable<EpisodeDto> episodes);
    }
}
