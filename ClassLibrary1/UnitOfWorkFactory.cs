using BotZeitNot.Domain.Interface;

namespace BotZeitNot.DAL
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWorkFactory(ApplicationDbContext context)
        {
            _context = context;
        }

        public IUnitOfWork Create()
        {
            return new UnitOfWork(_context);
        }
    }
}
