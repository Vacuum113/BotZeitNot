using BotZeitNot.DAL.Domain.Entity;
using BotZeitNot.DAL.Domain.Repositories.SpecificStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BotZeitNot.DAL.Domain.Repositories
{
    public class SeriesRepository : Repository<Series, int>, ISeriesRepository
    {

        public SeriesRepository(ApplicationDbContext context) : base(context)
        { }

        public IEnumerable<Series> GetByNameAllMatchSeries(string name)
        {
            Expression<Func<Series, bool>> predicate = a => (a.NameRu.StartsWith(name) || a.NameEn.StartsWith(name));

            return Table.
                Where(predicate).
                Take(7);
        }

        public Series GetByNameRuSeries(string nameRu)
        {
            return Table
                    .FirstOrDefault(s => s.NameRu == nameRu);
        }
    }
}