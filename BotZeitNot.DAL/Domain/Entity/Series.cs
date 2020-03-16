using BotZeitNot.Domain.Base;

namespace BotZeitNot.DAL.Domain.Entity
{
    public class Series : BaseEntity
    {
        public string NameRu { get; set; }

        public string NameEn { get; set; }

        public int SeasonsCount { get; set; }

        public string Link { get; set; }

        public bool IsCompleted { get; set; }
    }
}
