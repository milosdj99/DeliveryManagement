using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UserApi.Dto;

namespace UserApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {

        private readonly UserService _service;
        public UserController(UserService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Login([FromBody] UserLoginDto user)
        {
            if (_service.Login(user) == "NOT_APPROVED")
            {
                return BadRequest("Vas dostavljacki nalog nije verifikovan!");

            }
            else if (_service.Login(user) != null)
            {
                TokenModel token = new TokenModel();

                token.Value = _service.Login(user);

                return Ok(token);

            }
            else
            {
                return BadRequest("Pogresan e-mail ili lozinka!");
            }
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] UserRegisterDto user)
        {
            user.Id = new Guid();
            var userNew = _service.Register(user);
            if (userNew != null)
            {
                //return CreatedAtAction(nameof(GetUserById), new { id = userNew.Id }, userNew);
                return Ok(userNew);

            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("modify-profile/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ModifyProfile([FromRoute] Guid id, [FromBody] UserRegisterDto user)
        {
            if (_service.ModifyUser(id, user))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }


        }

        [HttpPut("change-picture/{id}"), DisableRequestSizeLimit]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ChangePicture([FromRoute] Guid id)
        {
            var file = Request.Form.Files.FirstOrDefault();
           

            if (_service.ChangePicture(id, file))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }

           
        }

        [HttpGet("picture/{id}")]
        public IActionResult GetPicture([FromRoute] Guid id)
        {
            TokenModel t = new TokenModel()
            {
                Value = _service.GetPicture(id)
            };
            return Ok(t);
        }

        [HttpPost("facebook-login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult FacebookLogin([FromBody] UserRegisterDto user)
        {

            TokenModel token = new TokenModel();

            token.Value = _service.FacebookLogin(user);

            return Ok(token);
        }


        [HttpGet("user/{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult GetUserById([FromRoute] Guid id)
        {

            return Ok(_service.GetUserById(id));
        }

        #region Admin

        [Authorize(Roles = "admin")]
        [HttpGet("admin/all-deliverers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAllDeliverers()
        {
            return Ok(_service.GetAllDeliverers());
        }

        [Authorize(Roles = "admin")]
        [HttpGet("admin/change-state/{id}/{state}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult ChangeDelivererState([FromRoute] Guid id, [FromRoute] string state)
        {
            _service.ChangeState(id, state);
            return Ok();
        }


        #endregion
    }
}
