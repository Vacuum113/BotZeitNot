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
            Expression<Func<Series, bool>> predicateRu = a => a.NameRu.ToLower().StartsWith(name.ToLower());
            Expression<Func<Series, bool>> predicateEn = a => a.NameEn.ToLower().StartsWith(name.ToLower());

            return Table.
                Where(predicateRu).
                Where(predicateEn).
                Take(7);
        }

        public Series GetByNameRuSeries(string nameRu)
        {
            return Table.FirstOrDefault(s => s.NameRu == nameRu);
        }
    }
}