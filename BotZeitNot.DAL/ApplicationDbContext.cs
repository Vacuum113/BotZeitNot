using BotZeitNot.DAL.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace BotZeitNot.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) => base.OnModelCreating(modelBuilder);

        public DbSet<User> Users { get; set; }

        public DbSet<Series> Series { get; set; }

        public DbSet<SubscriptionSeries> SubscriptionSeries { get; set; }
    }
}
