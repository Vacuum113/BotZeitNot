using BotZeitNot.DAL.Domain.Entity;
using BotZeitNot.DAL.Domain.Repositories.SpecificStorage;
using System.Collections.Generic;
using System.Linq;

namespace BotZeitNot.DAL.Domain.Repositories
{
    public class SubSeriesRepository : Repository<SubscriptionSeries, int>, ISubSeriesRepository
    {
        public SubSeriesRepository(ApplicationDbContext context) : base(context)
        { }

        public IEnumerable<string> GetAllSeriesNameByChatId(long chatId)
        {
            return Table.
                Where(ss => ss.ChatId == chatId).
                Select(ss => ss.SeriesNameRu);
        }

        public void CancelSubscription(long chatId, string seriesNameRu)
        {
            SubscriptionSeries series = Table.
                FirstOrDefault
                (
                ss =>
                ss.ChatId == chatId &&
                ss.SeriesNameRu == seriesNameRu
                );

            Table.Remove(series);
        }

        public long[] GetChatIdBySeriesNameRu(string titleSeries)
        {
            return Table
              .Where(ss => ss.SeriesNameRu == titleSeries)
              .Select(ss => ss.ChatId)
              .ToArray();
        }

        public void CancelSubscriptionFromAll(int chatId)
        {
            Table.RemoveRange(Table.Where(ss => ss.ChatId == chatId));
        }

        public bool IsUserSubscribedToSeries(int chatId, string nameRu)
        {
            var subSer = Table.
                FirstOrDefault
                (
                ss =>
                ss.ChatId == chatId &&
                ss.SeriesNameRu == nameRu
                );

            return subSer != default ? true : false;
        }
    }
}
