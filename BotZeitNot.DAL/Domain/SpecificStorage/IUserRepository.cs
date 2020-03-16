using BotZeitNot.DAL.Domain.Entity;

namespace BotZeitNot.DAL.Domain.Repositories.SpecificStorage
{
    public interface IUserRepository
    {
        public bool ContainsUserByTelegramId(int id);
    }
}
