using BotZeitNot.DAL.Domain.Entity;

namespace BotZeitNot.DAL.Domain.Repositories
{
    public class UserRepository : Repository<User, int>
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        { }
    }
}
