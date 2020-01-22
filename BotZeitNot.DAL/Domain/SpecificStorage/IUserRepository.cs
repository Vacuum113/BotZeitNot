using BotZeitNot.DAL.Domain.Entity;

namespace BotZeitNot.DAL.Domain.Repositories.SpecificStorage
{
    public interface IUserRepository
    {
        public bool ContainsUserByTelegramId(int id);

        public User GetUserAndSeriesByTelegramId(int id);

        public void SubscriprionOnSeries(Series series, User user);

        public void AddRangeNewSubSeries(long[] chatIdArray, string titleRu);
    }
}
