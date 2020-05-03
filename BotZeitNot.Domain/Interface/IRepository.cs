﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BotZeitNot.Domain.Interface
{
    public interface IRepository<T, in TKey> where T : class, IEntity<TKey>
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);

        T GetById(TKey id);

        IEnumerable<T> All();
        IEnumerable<T> Where(Expression<Func<T, bool>> where);
    }
}
