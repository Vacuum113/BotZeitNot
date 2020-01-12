using BotZeitNot.DAL.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BotZeitNot.DAL.Domain.Repositories
{
    public class UserRepository : Repository<User, int>
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        { }

        public bool ContainsUserByTelegramId(int id)
        {
            User user = Table.FirstOrDefault(u => u.TelegramId == id);
            return user.TelegramId != 0 ? true : false;
        }

        public User GetUserAndSeriesByTelegramId(int id)
        {
            return Table
                .Include(u => u.SeriesUser)
                .ThenInclude(su => su.Series)
                .FirstOrDefault(u => u.TelegramId == id);
        }

        public bool SubscriprionOnSeries(Series series, User user)
        {
            user.SeriesUser.Add(new SeriesUser { Series = series });
            Table.Add(user);

            int seriesId = user.SeriesUser.First(su => su.Series.NameRu == series.NameRu).SeriesId;

            return seriesId != 0 ? true : false;
        }
    }
}
