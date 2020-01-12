using BotZeitNot.Domain.Base;

namespace BotZeitNot.DAL.Domain.Entity
{
    public class Episode : BaseEntity
    {
        public int Number { get; set; }

        public string TitleRu { get; set; }

        public string TitleEn { get; set; }

        public string Link { get; set; }

        public string Rating { get; set; }

        public int SeasonId { get; set; }

        public Season Season { get; set; }
    }
}