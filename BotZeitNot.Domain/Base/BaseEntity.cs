using BotZeitNot.Domain.Interface;

namespace BotZeitNot.Domain.Base
{
    public class BaseEntity : IEntity<int>
    {
        public int Id { get; set; }
    }
}
