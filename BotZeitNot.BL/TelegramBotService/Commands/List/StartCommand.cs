using BotZeitNot.DAL;
using BotZeitNot.DAL.Domain.Repositories;
using BotZeitNot.Domain.Interface;
using System.Threading.Tasks;
using Telegram.Bot;
using Message = Telegram.Bot.Types.Message;
using User = BotZeitNot.DAL.Domain.Entity.User;


namespace BotZeitNot.BL.TelegramBotService.Commands.List
{
    public class StartCommand : Command
    {
        public override string Name => "/start";

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly UserRepository _userRepository;
        private TelegramBotClient _client;

        public StartCommand(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _userRepository = ((UnitOfWork)unitOfWorkFactory.Create()).Users;
        }

        public override async Task Execute(Message message, TelegramBotClient client)
        {
            _client = client;

            using (var unitOfWork = _unitOfWorkFactory.Create())
            {
                if (IsUserContainsInDb(message).Result)
                {
                    return;
                }

                _userRepository.Add(new User
                {
                    ChatId = message.Chat.Id,
                    TelegramId = message.From.Id,
                    FirstName = message.From.FirstName,
                    UserName = message.From.Username,
                    LastName = message.From.LastName
                });

                unitOfWork.Save();
            }
            await SendStartMessage(message);
        }


        private async Task<bool> IsUserContainsInDb(Message message)
        {
            if (!_userRepository.ContainsUserByTelegramId(message.From.Id)) 
                return false;
            const string errorMessage = "Вы уже использовали команду /start ";

            await _client.SendTextMessageAsync(message.Chat.Id, errorMessage);
            return true;
        }

        private async Task SendStartMessage(Message message)
        {
            const string startMessage = "Напишите нам, на рассылку " +
                                        "каких сериалов вы хотите подписаться.\n" +
                                        "Введите: /search <Сериал для поиска>. Без ковычек.";

            await _client.SendTextMessageAsync(message.Chat.Id, startMessage);
        }

    }
}
