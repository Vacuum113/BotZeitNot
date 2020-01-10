using BotZeitNot.DAL;
using BotZeitNot.DAL.Loader;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace BotZeitNot.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewController : ControllerBase
    {
        private IMapper _mapper;

        public NewController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpGet]
        public string Get()
        {
            return "kek";
        }
    }
}
