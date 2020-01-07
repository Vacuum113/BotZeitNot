using BotZeitNot.Domain.Map;
using BotZeitNot.DAL.Domain.Entity;
using BotZeitNot.Domain.Base;

namespace BotZeitNot.Shared.Dto
{
    [AutoMap(typeof(User))]
    public class UserDto : BaseDto
    {
        public int TelegramId { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
