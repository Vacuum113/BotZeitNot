using BotZeitNot.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotZeitNot.Domain.Base
{
    public class BaseDto : IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
