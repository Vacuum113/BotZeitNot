using BotZeitNot.DAL.Domain.Entity;
using BotZeitNot.DAL.Domain.Repositories;
using BotZeitNot.Domain.Interface;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotZeitNot.BL.TelegramBotService.Commands.CommandList.List
{
    class SearchCommand : Command
    {
        public override string Name => "/search";

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        private readonly SeriesRepository _seriesRepository;

        public SearchCommand(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;

            _seriesRepository = unitOfWorkFactory.Create().GetRepository<Series, int>() as SeriesRepository;
        }

        public async override void Execute(Message message, TelegramBotClient client)
        {
            using (IUnitOfWork unitOfWork = _unitOfWorkFactory.Create())
            {
                var searchString = new StringBuilder(message.Text);
                searchString.Remove(0, 8);

                var seriesName = _seriesRepository.GetByNameSeries(searchString.ToString());

                await client.SendTextMessageAsync(message.Chat.Id, seriesName);
            }
        }
    }
}
