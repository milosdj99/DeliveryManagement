using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Milos_Djukic_PR_21_2018.DTO;
using Milos_Djukic_PR_21_2018.Models;
using Milos_Djukic_PR_21_2018.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
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

        
        [HttpGet("articles")]
        public IActionResult GetArticles()
        {
            return Ok(_service.GetAllArticles());
        }


        #region Customer


        [Authorize(Roles = "customer")]
        [HttpPost("customer/add-order/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult AddOrder([FromBody] OrderDto order, [FromRoute] Guid id)
        {
            if(_service.AddOrder(order, id))
            {
                return Ok();
            } else
            {
                return BadRequest();
            }

        }


        [Authorize(Roles = "customer")]
        [HttpGet("customer/current-order/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCurrentOrderCustomer([FromRoute] Guid id)
        {
            
            return Ok(_service.GetCurrentOrderCustomer(id));
            
        }

        [Authorize(Roles = "customer")]
        [HttpGet("customer/previous-orders/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult CustomerOrders([FromRoute] Guid id)
        {
            return Ok(_service.GetCustomerOrders(id));
        }

        #endregion

        #region Deliverer

        [Authorize(Roles = "deliverer")]
        [HttpGet("deliverer/new-orders/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetNewOrders(Guid id)
        {
            return Ok(_service.GetNewOrdersDeliverer(id));
        }

        [Authorize(Roles = "deliverer")]
        [HttpGet("deliverer/previous-orders/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetDelivererOrders(Guid id)
        {
            return Ok(_service.GetDelivererOrders(id));
        }

        [Authorize(Roles = "deliverer")]
        [HttpGet("deliverer/current-order/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCurrentOrderDeliverer(Guid id)
        {
            return Ok(_service.GetCurrentOrderDeliverer(id));
        }

        [Authorize(Roles = "deliverer")]
        [HttpGet("deliverer/confirm-order/{id}/{orderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult ConfirmOrder(Guid id, Guid orderId)
        {
            if(_service.ConfirmOrder(id, orderId))
            {
                return Ok();
            }else
            {
                return BadRequest();
            }


        }

        #endregion


        #region Admin


        [Authorize(Roles = "admin")]
        [HttpPost("admin/add-article")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult AddArticle([FromBody]ArticleDTO article)
        {
            if (_service.AddArticle(article))
            {
                return Ok();
            } else
            {
                return BadRequest();
            }

            
        }


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

        [Authorize(Roles = "admin")]
        [HttpGet("admin/all-orders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAllOrders()
        {
            return Ok(_service.GetAllOrders());
        }

        

        #endregion

        #region Login/Register

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Login([FromBody]UserLoginDto user)
        {
            if (_service.Login(user) == "NOT_APPROVED")
            {
                return BadRequest("Vas dostavljacki nalog nije verifikovan!");

            }else if (_service.Login(user) != null)
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
        public ActionResult Register([FromBody]UserRegisterDto user)
        {
            user.Id = new Guid();
            var userNew = _service.Register(user);
            if (userNew != null)
            {
                return CreatedAtAction(nameof(GetUserById), new { id = userNew.Id }, userNew);
                

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

            var folderName = Path.Combine("Resources", "Images");

            var path = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            if(file.Length > 0)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var fullPath = Path.Combine(path, fileName);
                var dbPath = Path.Combine(folderName, fileName);

                using(var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                if(_service.ChangePicture(id, dbPath))
                {
                    return Ok();
                } else
                {
                    return BadRequest();
                }

               
            } else
            {
                return BadRequest();
            }
            
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

        #endregion







    }
}
