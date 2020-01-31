using System.Collections.Generic;

namespace BotZeitNot.DAL.Domain.Repositories.SpecificStorage
{
    public interface ISubSeriesRepository
    {
        public IEnumerable<string> GetAllSeriesNameByChatId(long chatId);

        public void CancelSubscription(long chatId, string seriesNameRu);

        public long[] GetChatIdBySeriesNameRu(string titleSeries);

        public void CancelSubscriptionFromAll(int chatId);

        public bool IsUserSubscribedToSeries(int chatId, string nameRu);
}
