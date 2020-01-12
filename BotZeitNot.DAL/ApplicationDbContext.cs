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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SeriesUser>()
                .HasKey(su => new { su.SeriesId, su.UserId });
            modelBuilder.Entity<SeriesUser>()
                .HasOne(su => su.Series)
                .WithMany(s => s.SeriesUser)
                .HasForeignKey(su => su.SeriesId);
            modelBuilder.Entity<SeriesUser>()
                .HasOne(su => su.User)
                .WithMany(u => u.SeriesUser)
                .HasForeignKey(su => su.UserId);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Series> Series { get; set; }

        public DbSet<SeriesUser> SeriesUsers { get; set; }

        public DbSet<Season> Seasons { get; set; }

        public DbSet<Episode> Episodes { get; set; }
    }
}
