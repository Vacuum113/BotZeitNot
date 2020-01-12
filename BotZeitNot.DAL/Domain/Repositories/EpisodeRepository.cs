using BotZeitNot.DAL.Domain.Entity;

namespace BotZeitNot.DAL.Domain.Repositories
{
    public class EpisodeRepository : Repository<Episode, int>
    {
        public EpisodeRepository(ApplicationDbContext context) : base(context)
        { }
    }
}
