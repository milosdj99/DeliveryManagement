using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {
       
        [HttpGet("getff")]
        public IActionResult GetFf()
        {
            return Ok("nesto");
        }
    }
}
