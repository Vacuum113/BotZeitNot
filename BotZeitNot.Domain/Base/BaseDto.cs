using BotZeitNot.Domain.Interface;

namespace BotZeitNot.Domain.Base
{
    public class BaseDto : IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
