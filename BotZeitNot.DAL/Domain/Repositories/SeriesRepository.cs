using BotZeitNot.DAL.Domain.Entity;
using BotZeitNot.DAL.Domain.Repositories.SpecificStorage;
using Microsoft.EntityFrameworkCore;
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
            Expression<Func<Series, bool>> predicate = a => (a.NameRu == name ||
                                                             a.NameEn == name);

            return Table.
                Where(predicate).
                Take(7);
        }

        public Series GetSeriesSeasonsAndEpisodesByRuName(string name)
        {
            return _context.Series.
                Include(s => s.Seasons).
                ThenInclude(s => s.Episodes).
                FirstOrDefault(a => a.NameRu == name);
        }

        public Series GetByNameRuSeries(string nameRu)
        {
            return Table
                    .First(s => s.NameRu == nameRu);
        }
    }
}