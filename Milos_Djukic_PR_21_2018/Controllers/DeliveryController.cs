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


        #region Customer


        [HttpPost("add-order/{id}")]
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

           

        [HttpGet("customer/current-order/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCurrentOrderCustomer([FromRoute] Guid id)
        {
            
            return Ok(_service.GetCurrentOrderCustomer(id));
            
        }

        [HttpGet("customer/previous-orders/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult CustomerOrders([FromRoute] Guid id)
        {
            return Ok(_service.GetCustomerOrders(id));
        }

        #endregion

        #region Deliverer

        [HttpGet("deliverer/new-orders/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetNewOrders(Guid id)
        {
            return Ok(_service.GetNewOrdersDeliverer(id));
        }


        [HttpGet("deliverer/previous-orders/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetDelivererOrders(Guid id)
        {
            return Ok(_service.GetDelivererOrders(id));
        }

        [HttpGet("deliverer/current-order/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCurrentOrderDeliverer(Guid id)
        {
            return Ok(_service.GetCurrentOrderDeliverer(id));
        }

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



        [HttpPost("admin/add-article")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult AddArticle([FromBody]ArticleDTO article)
        {
            _service.AddArticle(article);

            return Ok();
        }


        [HttpGet("admin/all-deliverers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAllDeliverers()
        {
            return Ok(_service.GetAllDeliverers());
        }

        [HttpGet("admin/accept-deliverer/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult AcceptDeliverer([FromRoute] Guid id)
        {
            _service.AcceptDeliverer(id);
            return Ok();
        }

        [HttpGet("/admin/all-orders")]
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

        [HttpPut("modify-profile/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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

        #endregion







    }
}
