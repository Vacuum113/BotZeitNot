using BotZeitNot.Domain.Base;

namespace BotZeitNot.DAL.Domain.Entity
{
    public class SubscriptionSeries : BaseEntity
    {
        public int TelegramUserId { get; set; }

        public string SeriesNameRu { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}
