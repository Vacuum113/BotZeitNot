using BotZeitNot.Domain.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BotZeitNot.DAL
{
    public class Repository<T, TPrimaryKey> : IRepository<T, TPrimaryKey> 
        where T : class, IEntity<TPrimaryKey>
    {
        private readonly ApplicationDbContext _context;
        public DbSet<T> Table { get; set; }

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            Table = context.Set<T>();
        }

        public T GetById(TPrimaryKey id)
        {
            return Table.FirstOrDefault(c => c.Id.Equals(id));
        }

        public TPrimaryKey Add(T entity)
        {
            Table.Add(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public void Update(T entity)
        {
            Table.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(T entity)
        {
            Table.Remove(entity);
            _context.SaveChanges();
        }

        public IEnumerable<T> All()
        {
            return Table.ToList();
        }

        public IEnumerable<T> Where(Expression<Func<T, bool>> where)
        {
            return Table.Where(where).ToList();
        }

    }
}
