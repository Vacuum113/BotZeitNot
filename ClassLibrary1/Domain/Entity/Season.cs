using BotZeitNot.Domain.Base;
using System.Collections.Generic;

namespace BotZeitNot.DAL.Domain.Entity
{
    public class Season : BaseEntity
    {
        public int Name { get; set; }

        public ICollection<Episode> Episodes { get; set; }

        public int SeriesId { get; set; }

        public Series Series { get; set; }

    }
}