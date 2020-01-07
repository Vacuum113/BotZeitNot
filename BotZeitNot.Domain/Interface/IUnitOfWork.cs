using System;

namespace BotZeitNot.Domain.Interface
{
    public interface IUnitOfWork : IDisposable 
    {
        public IRepository<T, TKey> GetRepository<T, TKey>() where T : class, IEntity<TKey>;

        public void Save();

    }
}
