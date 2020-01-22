using BotZeitNot.DAL;
using BotZeitNot.DAL.Domain.Repositories;
using BotZeitNot.Domain.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = BotZeitNot.DAL.Domain.Entity.User;

namespace BotZeitNot.BL.TelegramBotService.Commands.List
{
    public class CancelCommand : Command
    {
        public override string Name => "/cancel";


        private UserRepository _userRepository;
        private TelegramBotClient _client;
        private Message _message;

        public CancelCommand(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _userRepository = ((UnitOfWork)unitOfWorkFactory.Create()).Users;
        }

        public async override Task Execute(Message message, TelegramBotClient client)
        {
            _client = client;
            _message = message;

            var user = _userRepository.GetUserAndSeriesByTelegramId(message.From.Id);
            if (CheckUser(user).Result)
            {
                return;
            }

            List<string> series = user.SubscriptionSeries
                .Select(s => s.SeriesNameRu)
                .ToList();

            if (CheckSubSeries(series).Result)
            {
                return;
            }

            await SendCancelButtons(series);
        }


        private async Task<bool> CheckUser(User user)
        {
            if (user == default)
            {
                string errorMessage = "Пользователь не найден, " +
                                      "попробуйте начать с команды /start";

                await _client.SendTextMessageAsync(_message.Chat.Id, errorMessage);
                return true;
            }
            return false;
        }

        private async Task<bool> CheckSubSeries(List<string> series)
        {
            if (series == null || series.Count == 0)
            {
                string errorMessage = "У вас еще нет подписок.\n" +
                                      "Найти и добавить сериалы " +
                                      "можно с помощью команды /search";

                await _client.SendTextMessageAsync(_message.Chat.Id, errorMessage);
                return true;
            }
            return false;
        }

        private async Task SendCancelButtons(List<string> series)
        {
            string cancelMessage = "Выберите, от рассылки какого " +
                                   "сериала вы хотите отписаться";

            var buttons = series.Select(s => new InlineKeyboardButton()
            {
                Text = s,
                CallbackData = "Cancel/" + s
            });

            await _client.SendTextMessageAsync
                (
                _message.Chat.Id,
                cancelMessage,
                replyMarkup: new InlineKeyboardMarkup(buttons)
                );
        }
    }
}
