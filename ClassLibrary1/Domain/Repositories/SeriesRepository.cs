using BotZeitNot.DAL.Domain.Entity;
using System.Linq;

namespace BotZeitNot.DAL.Domain.Repositories
{
    public class SeriesRepository : Repository<Series, int>
    {

        public SeriesRepository(ApplicationDbContext context) : base(context)
        { }

        public string GetByNameSeries(string name)
        {
            return Table.First(a => a.NameRu == name).NameRu;
        }
    }
}