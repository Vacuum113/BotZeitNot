using BotZeitNot.BL.TelegramBotService;
using BotZeitNot.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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
        public IActionResult TelegramUpdates(Update update)
        {
            _botService.Run(update);

            return Ok();
        }

        [HttpPost("NewEpisodes")]
        public IActionResult Post(IEnumerable<Episode> episodes)
        {

        }
    }
}