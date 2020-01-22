using BotZeitNot.DAL.Domain.Entity;
using BotZeitNot.DAL.Domain.Repositories.SpecificStorage;

namespace BotZeitNot.DAL.Domain.Repositories
{
    public class SeasonRepository : Repository<Season, int>, ISeasonRepository
    {
        public SeasonRepository(ApplicationDbContext context) : base(context)
        { }

    }
}
