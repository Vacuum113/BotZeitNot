namespace BotZeitNot.DAL.Domain.Entity
{
    public class SeriesUser
    {
        public int UserId { get; set; }

        public int SeriesId { get; set; }

        public User User { get; set; }

        public Series Series { get; set; }

    }
}
