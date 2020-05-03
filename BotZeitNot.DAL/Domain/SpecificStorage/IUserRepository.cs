namespace BotZeitNot.DAL.Domain.SpecificStorage
{
    public interface IUserRepository
    {
        public bool ContainsUserByTelegramId(int id);
    }
}
