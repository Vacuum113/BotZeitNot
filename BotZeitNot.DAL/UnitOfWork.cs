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
        private SeasonRepository _seasonRepository;
        private EpisodeRepository _episodeRepository;

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

        public SeasonRepository Seasons {
            get {
                if (_seasonRepository == null)
                {
                    _seasonRepository = new SeasonRepository(_context);
                }
                return _seasonRepository;
            }
        }

        public EpisodeRepository Episodes {
            get {
                if (_episodeRepository == null)
                {
                    _episodeRepository = new EpisodeRepository(_context);
                }
                return _episodeRepository;
            }
        }


        public IRepository<T, TPrimaryKey> GetRepository<T, TPrimaryKey>() where T : class, IEntity<TPrimaryKey>
        {
            return new Repository<T, TPrimaryKey>(_context);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async void SaveAsync()
        {
            await _context.SaveChangesAsync();
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
