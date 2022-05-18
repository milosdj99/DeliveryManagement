using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Milos_Djukic_PR_21_2018.DTO;
using Milos_Djukic_PR_21_2018.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Milos_Djukic_PR_21_2018
{
    [Route("api/delivery")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly DeliveryService _service;
        public DeliveryController(DeliveryService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody]UserLoginDto user)
        {
            if(_service.Login(user) != null)
            {
                return Ok(_service.Login(user));

            } else
            {
                return BadRequest();
            }
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody]UserRegisterDto user)
        {
            if (_service.Register(user))
            {
                return Ok(user);

            }
            else
            {
                return BadRequest();
            }
        }





    }
}
