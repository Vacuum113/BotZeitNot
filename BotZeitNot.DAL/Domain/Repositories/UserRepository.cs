using BotZeitNot.DAL.Domain.Entity;
using System.Linq;

namespace BotZeitNot.DAL.Domain.Repositories
{
    public class UserRepository : Repository<User, int>
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        { }

        public bool GetByTelegramId(int id)
        {
            User user = Table.FirstOrDefault(u => u.TelegramId == id);
            return user.TelegramId != 0 ? true : false;
        }
    }
}
