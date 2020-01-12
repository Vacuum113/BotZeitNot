using BotZeitNot.DAL.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BotZeitNot.DAL.Domain.Repositories
{
    public class SeriesRepository : Repository<Series, int>
    {

        public SeriesRepository(ApplicationDbContext context) : base(context)
        { }

        public IEnumerable<Series> GetByNameAllMatchSeries(string name)
        {
            Expression<Func<Series, bool>> predicate = a => (a.NameRu.ToLowerInvariant() == name.ToLowerInvariant() ||
                                                             a.NameEn.ToLowerInvariant() == name.ToLowerInvariant()) &&
                                                             !a.IsCompleted;

            return Table.
                Where(predicate).
                Take(7).
                ToList();
        }

        public Series GetSeriesSeasonsAndEpisodesByRuName(string name)
        {
            return _context.Series.
                Include(s => s.Seasons).
                ThenInclude(s => s.Episodes).
                FirstOrDefault(a=>a.NameRu == name);
        }
    }
}