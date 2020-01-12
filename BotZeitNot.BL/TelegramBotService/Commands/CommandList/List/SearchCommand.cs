using BotZeitNot.DAL;
using BotZeitNot.DAL.Domain.Entity;
using BotZeitNot.DAL.Domain.Repositories;
using BotZeitNot.Domain.Interface;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

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

            _seriesRepository = ((UnitOfWork)unitOfWorkFactory.Create()).Series;
        }

        public async override void Execute(Message message, TelegramBotClient client)
        {
            if (message.Text.Length < 8)
            {
                return;
            }

            string text = message.Text.Remove(0, 8).TrimStart();

            if (text == "")
            {
                return;
            }

            IEnumerable<Series> series = _seriesRepository.GetByNameAllMatchSeries(text);

            var buttons = new List<InlineKeyboardButton>();
            foreach (var item in series)
            {
                var inlineKeyboardButton = new InlineKeyboardButton()
                {
                    Text = item.NameRu,
                    CallbackData = "Search/" + item.NameRu
                };
                buttons.Add(inlineKeyboardButton);
            }

            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Найденые сериалы: ",
                replyMarkup: new InlineKeyboardMarkup(buttons)
                );
        }
    }
}
