using BotZeitNot.DAL.Domain.Entity;
using BotZeitNot.DAL.Domain.Repositories.SpecificStorage;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BotZeitNot.DAL.Domain.Repositories
{
    public class UserRepository : Repository<User, int>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        { }

        public bool ContainsUserByTelegramId(int id)
        {
            User user = Table.FirstOrDefault(u => u.TelegramId == id);
            return user != default ? true : false;
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
                ChatId = user.ChatId,
                SeriesNameRu = series.NameRu
            });

            Table.Update(user);
        }

        public void AddRangeNewSubSeries(long[] chatIdArray, string titleRu)
        {
            List<SubscriptionSeries> episodes = chatIdArray.
                Select(c => new SubscriptionSeries
                {
                    ChatId = c,
                    SeriesNameRu = titleRu
                }).
                ToList();

            List<User> users = new List<User>();
            foreach (var chatId in chatIdArray)
            {
                User user = Table.FirstOrDefault(u => u.ChatId == chatId);

                if (user != default)
                {
                    users.Add(user);
                }
                //else
                //{

                //}
            }
            Table.AddRange(users);
        }
    }

}