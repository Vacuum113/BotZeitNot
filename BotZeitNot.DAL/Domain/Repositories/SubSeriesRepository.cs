using BotZeitNot.DAL.Domain.Entity;
using System.Collections.Generic;
using System.Linq;

namespace BotZeitNot.DAL.Domain.Repositories
{
    public class SubSeriesRepository : Repository<SubscriptionSeries, int>
    {
        public SubSeriesRepository(ApplicationDbContext context) : base(context)
        { }

        public IEnumerable<string> GetAllSeriesNameByTelegramId(int telegramId)
        {
            return Table.
                Where(ss => ss.TelegramUserId == telegramId).
                Select(ss => ss.SeriesNameRu);
        }

        public void CancelSubscription(int telegramId, string seriesNameRu)
        {
            SubscriptionSeries series = Table.
                FirstOrDefault
                (
                ss =>
                ss.TelegramUserId == telegramId &&
                ss.SeriesNameRu == seriesNameRu
                );

            Table.Remove(series);
        }
    }
}
