using BotZeitNot.Domain.Base;
using System.Collections.Generic;

namespace BotZeitNot.DAL.Domain.Entity
{
    public class User : BaseEntity
    {
        public long ChatId { get; set; }

        public int TelegramId { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ICollection<SubscriptionSeries> SubscriptionSeries { get; set; }
    }
}
