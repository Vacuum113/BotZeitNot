using BotZeitNot.BL.TelegramBotService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;


namespace BotZeitNot.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UpdateController : ControllerBase
    {
        private readonly ILogger<UpdateController> _logger;

        private readonly ITelegramBotService _botService;

        public UpdateController(ILogger<UpdateController> logger, ITelegramBotService botService)
        {
            _logger = logger;
            _botService = botService;
        }

        [HttpGet]
        public string Get()
        {
            return "kek";
        }

        [HttpPost]
        public IActionResult Post(Update update)
        {
            _botService.ExecuteCommand(update);

            return Ok();
        }
    }
}