using BotZeitNot.DAL;
using BotZeitNot.DAL.Domain.Entity;
using BotZeitNot.DAL.Domain.Repositories;
using BotZeitNot.Domain.Interface;
using Telegram.Bot;
using Message = Telegram.Bot.Types.Message;


namespace BotZeitNot.BL.TelegramBotService.Commands.CommandList.List
{
    public class StartCommand : Command
    {
        public override string Name => "/start";

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        private readonly UserRepository _userRepository;

        public StartCommand(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _userRepository = ((UnitOfWork)unitOfWorkFactory.Create()).Users;
        }

        public async override void Execute(Message message, TelegramBotClient client)
        {
            using (IUnitOfWork unitOfWork = _unitOfWorkFactory.Create())
            {
                if(_userRepository.GetByTelegramId(message.From.Id))
                {
                    return;
                }

                _userRepository.Add(new User
                {
                    TelegramId = message.From.Id,
                    FirstName = message.From.FirstName ?? null,
                    UserName = message.From.Username,
                    LastName = message.From.LastName ?? null
                });

                unitOfWork.Save();

                await client.SendTextMessageAsync(message.Chat.Id, "Напишите нам, на рассылку каких сериалов вы хотите подписаться." +
                    "Введите /search \"Сериал для поиска\"");
            }
        }
    }
}
