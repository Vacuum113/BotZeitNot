using BotZeitNot.Domain.Base;
using System.Collections.Generic;

namespace BotZeitNot.DAL.Domain.Entity
{
    public class Series : BaseEntity
    {
        public string NameRu { get; set; }

        public string NameEu { get; set; }

        public int SeasonsCount { get; set; }

        public string Link { get; set; }

        public bool IsCompleted { get; set; }

        public ICollection<SeriesUser> SeriesUser { get; set; }

        public ICollection<Season> Seasons { get; set; }
    }
}
