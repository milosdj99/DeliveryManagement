using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Milos_Djukic_PR_21_2018.DTO;
using Milos_Djukic_PR_21_2018.Models;
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

        [HttpGet("users/{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult GetUserById([FromRoute] Guid id)
        {
            
            return Ok(_service.GetUserById(id));
        }

        [HttpPut("modify-profile/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult ModifyProfile([FromRoute]Guid id, [FromBody] UserRegisterDto user)
        {
            _service.ModifyUser(id, user);

            return Ok();
        }

        [HttpGet("articles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAllArticles()
        {
            return Ok(_service.GetAllArticles());
        }

        [HttpPost("add-order/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult AddOrder([FromBody] OrderDto order, [FromRoute]Guid id)
        {
            _service.AddOrder(order, id);

            return Ok();
        }

        [HttpGet("current-order/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCurrentOrder([FromRoute] Guid id)
        {
            if(_service.GetCurrentOrder(id) != null)
            {
                return Ok(_service.GetCurrentOrder(id));
            } else
            {
                return NotFound();
            }
        }

        



        [HttpPost("add-article")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult AddArticle([FromBody]ArticleDTO article)
        {
            _service.AddArticle(article);

            return Ok();
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Login([FromBody]UserLoginDto user)
        {
            if(_service.Login(user) != null)
            {
                TokenModel token = new TokenModel();
                
                token.Value = _service.Login(user);
                
                return Ok(token);

            } else
            {
                return BadRequest();
            }
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody]UserRegisterDto user)
        {
            var userNew = _service.Register(user);
            if (userNew != null)
            {
                //return CreatedAtAction(nameof(GetUserById), new { id = userNew.Id }, userNew);
                return Ok();

            }
            else
            {
                return BadRequest();
            }
        }

       





    }
}
