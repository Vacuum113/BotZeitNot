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
                .Include(u => u.SubscriptionSeries)
                .FirstOrDefault(u => u.TelegramId == id);
        }

        public void SubscriprionOnSeries(Series series, User user)
        {
            user.SubscriptionSeries.Add(new SubscriptionSeries 
            { 
                TelegramUserId = user.TelegramId,
                SeriesNameRu = series.NameRu
            });

            Table.Update(user);
        }
    }
}
