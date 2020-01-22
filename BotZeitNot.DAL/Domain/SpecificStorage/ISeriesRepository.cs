using BotZeitNot.DAL.Domain.Entity;
using System.Collections.Generic;

namespace BotZeitNot.DAL.Domain.Repositories.SpecificStorage
{
    public interface ISeriesRepository
    {
        public IEnumerable<Series> GetByNameAllMatchSeries(string name);

        public Series GetSeriesSeasonsAndEpisodesByRuName(string name);

        public Series GetByNameRuSeries(string nameRu);
    }
}
