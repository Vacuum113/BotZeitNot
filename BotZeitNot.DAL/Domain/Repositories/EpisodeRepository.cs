using BotZeitNot.DAL.Domain.Entity;
using BotZeitNot.DAL.Domain.Repositories.SpecificStorage;

namespace BotZeitNot.DAL.Domain.Repositories
{
    public class EpisodeRepository : Repository<Episode, int>, IEpisodeRepository
    {
        public EpisodeRepository(ApplicationDbContext context) : base(context)
        { }
    }
}
