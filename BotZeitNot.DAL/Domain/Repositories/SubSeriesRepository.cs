using BotZeitNot.DAL.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BotZeitNot.DAL.Domain.SpecificStorage;

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
            Expression<Func<SubscriptionSeries, bool>> predicat = ss => ss.ChatId == chatId && ss.SeriesNameRu == seriesNameRu;
            var series = Table.FirstOrDefault(predicat);

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

        public bool IsUserSubscribedToSeries(long chatId, string nameRu)
        {
            Expression<Func<SubscriptionSeries, bool>> predicate = ss => ss.ChatId == chatId && ss.SeriesNameRu == nameRu;
            var subSer = Table.FirstOrDefault(predicate);

            return subSer != default;
        }

        public void AddSubscription(long chatId, string nameRu)
        {
            var user = _context.Users.
                Include(u => u.SubscriptionSeries).
                FirstOrDefault(u=>u.ChatId == chatId);

            user?.SubscriptionSeries.Add(new SubscriptionSeries
            {
                ChatId = chatId,
                SeriesNameRu = nameRu
            });
        }
    }
}
