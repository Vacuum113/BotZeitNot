using BotZeitNot.DAL;
using BotZeitNot.DAL.Domain.Entity;
using BotZeitNot.DAL.Domain.Repositories;
using BotZeitNot.Domain.Interface;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotZeitNot.BL.TelegramBotService.Commands.List
{ 
    public class CancelCommand : Command
    {
        public override string Name => "/cancel";


        private readonly UserRepository _userRepository;

        public CancelCommand(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _userRepository = ((UnitOfWork)unitOfWorkFactory.Create()).Users;
        }

        public async override void Execute(Message message, TelegramBotClient client)
        {
            var user = _userRepository.GetUserAndSeriesByTelegramId(message.From.Id);

            List<string> series = user.SubscriptionSeries
                .Select(s=>s.SeriesNameRu)
                .ToList();

            string cancelMessage = "Выберите от рассылки какого " +
                                   "сериала вы хотите отписаться";

            var buttons = series.Select(s => new InlineKeyboardButton()
            {
                Text = s,
                CallbackData = "Cancel/" + s
            });

            await client.SendTextMessageAsync
                (
                message.Chat.Id,
                cancelMessage,
                replyMarkup: new InlineKeyboardMarkup(buttons)
                );


        }
    }
}
