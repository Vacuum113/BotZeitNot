using BotZeitNot.DAL;
using BotZeitNot.DAL.Domain.Repositories;
using BotZeitNot.Domain.Interface;
using Microsoft.Extensions.Logging;
using System;
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


        private SubSeriesRepository _subSeriesRepository;
        private TelegramBotClient _client;
        private Message _message;

        public CancelCommand(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _subSeriesRepository = ((UnitOfWork)unitOfWorkFactory.Create()).SubSeries;
        }


        public async override Task Execute(Message message, TelegramBotClient client)
        {
            _client = client;
            _message = message;


            List<string> series = _subSeriesRepository.GetAllSeriesNameByChatId(message.Chat.Id).ToList();

            if (!IsSubSeriesValid(series).Result)
            {
                return;
            }   

            var buttons = MakeInlineKeyboardButtons(series);

            await SendCancelButtons(buttons);
        }


        private async Task<bool> IsUserValid(User user)
        {
            if (user == default)
            {
                string errorMessage = "Пользователь не найден, " +
                                      "попробуйте начать с команды /start";

                await _client.SendTextMessageAsync(_message.Chat.Id, errorMessage);
                return false;
            }
            return true;
        }

        private async Task<bool> IsSubSeriesValid(List<string> series)
        {
            if (series == null || series.Count == 0)
            {
                string errorMessage = "У вас еще нет подписок.\n" +
                                      "Найти и добавить сериалы " +
                                      "можно с помощью команды /search";

                await _client.SendTextMessageAsync(_message.Chat.Id, errorMessage);
                return false;
            }
            return true;
        }

        private List<List<InlineKeyboardButton>> MakeInlineKeyboardButtons(List<string> series)
        {
            List<InlineKeyboardButton> buttonsList = series.
                Select(s => new InlineKeyboardButton()
                {
                    Text = s,
                    CallbackData = "Cancel/" + s
                }).
                ToList();


            var buttonsKeyboard = new List<List<InlineKeyboardButton>>();
            List<InlineKeyboardButton> localList = new List<InlineKeyboardButton>();

            foreach (var item in buttonsList)
            {
                localList.Add(item);
                if (localList.Count == 2)
                {
                    buttonsKeyboard.Add(localList);
                    localList = new List<InlineKeyboardButton>();
                }
            }

            if(series.Count % 2 != 0)
            {
                buttonsKeyboard.Add(new List<InlineKeyboardButton> 
                {
                    buttonsList.LastOrDefault() 
                });
            }

            buttonsKeyboard.Add(new List<InlineKeyboardButton>
            {
                new InlineKeyboardButton
                {
                    Text = "Cancel All",
                    CallbackData = "CancelAll"
                }
            });
            return buttonsKeyboard;
        }


        private async Task SendCancelButtons(List<List<InlineKeyboardButton>> buttons)
        {
            string cancelMessage = "Выберите, от рассылки какого " +
                                   "сериала вы хотите отписаться";

            await _client.SendTextMessageAsync
                (
                _message.Chat.Id,
                cancelMessage,
                replyMarkup: new InlineKeyboardMarkup(buttons)
                );
        }
    }
}
