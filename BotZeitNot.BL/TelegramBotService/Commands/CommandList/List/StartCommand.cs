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
            _userRepository = unitOfWorkFactory.Create().GetRepository<User, int>() as UserRepository;
        }

        public async override void Execute(Message message, TelegramBotClient client)
        {
            using (IUnitOfWork unitOfWork = _unitOfWorkFactory.Create())
            {
                _userRepository.Add(new User
                {
                    TelegramId = message.From.Id,
                    FirstName = message.From.FirstName,
                    UserName = message.From.Username,
                    LastName = message.From.Username
                });

                await client.SendTextMessageAsync(message.Chat.Id, "Напишите нам, на рассылку каких сериалов вы хотите подписаться.");

                unitOfWork.Save();
            }
        }
    }
}
