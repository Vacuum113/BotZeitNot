using BotZeitNot.DAL.Domain.Repositories;
using BotZeitNot.Domain.Interface;
using System;

namespace BotZeitNot.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private UserRepository _userRepository;
        private SeriesRepository _seriesRepository;
        private SubSeriesRepository _subSeriesRepository;

        private bool _disposed;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public UserRepository Users {
            get {
                if (_userRepository == null)
                {
                    _userRepository = new UserRepository(_context);
                }
                return _userRepository;
            }
        }

        public SeriesRepository Series {
            get {
                if (_seriesRepository == null)
                {
                    _seriesRepository = new SeriesRepository(_context);
                }
                return _seriesRepository;
            }
        }

        public SubSeriesRepository SubSeries {
            get {
                if (_subSeriesRepository == null)
                {
                    _subSeriesRepository = new SubSeriesRepository(_context);
                }
                return _subSeriesRepository;
            }
        }

        public IRepository<T, TPrimaryKey> GetRepository<T, TPrimaryKey>()
            where T : class, IEntity<TPrimaryKey>
        {
            return new Repository<T, TPrimaryKey>(_context);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                this._disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
