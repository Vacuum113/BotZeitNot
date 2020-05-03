using System.Collections.Generic;
using BotZeitNot.DAL.Domain.Entity;

namespace BotZeitNot.DAL.Domain.SpecificStorage
{
    public interface ISeriesRepository
    {
        public IEnumerable<Series> GetByNameAllMatchSeries(string name);

        public Series GetByNameRuSeries(string nameRu);
    }
}
